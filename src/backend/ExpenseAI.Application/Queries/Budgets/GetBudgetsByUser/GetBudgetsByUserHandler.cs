using Ardalis.Result;
using ExpenseAI.Application.Common;
using ExpenseAI.Application.Interfaces;
using ExpenseAI.Application.Queries.Budgets.DTOs;
using ExpenseAI.Domain.Entities;
using ExpenseAI.Domain.ValueObjects;
using CategoryVO = ExpenseAI.Domain.ValueObjects.Category;

namespace ExpenseAI.Application.Queries.Budgets.GetBudgetsByUser;

public class GetBudgetsByUserHandler : IQueryHandler<GetBudgetsByUserQuery, Common.PagedResult<BudgetDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBudgetsByUserHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Common.PagedResult<BudgetDto>>> Handle(GetBudgetsByUserQuery request, CancellationToken cancellationToken)
    {
        var budgets = await _unitOfWork.Budgets.GetByUserIdAsync(request.UserId, cancellationToken);

        var totalCount = budgets.Count;
        var skip = (request.Page - 1) * request.PageSize;
        var pagedBudgets = budgets
            .OrderByDescending(b => b.CreatedAt)
            .Skip(skip)
            .Take(request.PageSize)
            .ToList();

        // Get expenses for all budgets to calculate spent amounts
        var allExpenses = await _unitOfWork.Expenses.GetByUserIdAsync(request.UserId, cancellationToken);

        var budgetDtos = new List<BudgetDto>();

        foreach (var budget in pagedBudgets)
        {
            var categoryExpenses = allExpenses.Where(e =>
                e.Category.Name == budget.Category &&
                e.ExpenseDate >= budget.StartDate &&
                e.ExpenseDate <= budget.EndDate).ToList();

            var spentAmount = new Money(
                categoryExpenses.Sum(e => e.Amount.Amount),
                budget.Amount.Currency);

            var remainingAmount = new Money(
                budget.Amount.Amount - spentAmount.Amount,
                budget.Amount.Currency);

            var percentageUsed = budget.Amount.Amount > 0
                ? (spentAmount.Amount / budget.Amount.Amount) * 100
                : 0;

            var dto = new BudgetDto
            {
                Id = budget.Id,
                UserId = budget.UserId,
                Name = budget.Name,
                Description = budget.Description ?? string.Empty,
                Amount = budget.Amount,
                Category = GetCategoryByName(budget.Category),
                StartDate = budget.StartDate,
                EndDate = budget.EndDate,
                IsRecurring = budget.Period != BudgetPeriod.Monthly,
                RecurrencePattern = budget.Period.ToString().ToLowerInvariant(),
                SpentAmount = spentAmount,
                RemainingAmount = remainingAmount,
                PercentageUsed = percentageUsed,
                IsOverBudget = spentAmount.Amount > budget.Amount.Amount,
                CreatedAt = budget.CreatedAt,
                UpdatedAt = budget.UpdatedAt
            };

            budgetDtos.Add(dto);
        }

        var result = new Common.PagedResult<BudgetDto>
        {
            Items = budgetDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };

        return Result.Success(result);
    }

    private static CategoryVO GetCategoryByName(string categoryName)
    {
        return CategoryVO.SystemCategories.FirstOrDefault(c =>
            c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase)) ?? CategoryVO.Uncategorized;
    }
}
