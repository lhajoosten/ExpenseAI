using Ardalis.Result;
using ExpenseAI.Application.Common;
using ExpenseAI.Application.Interfaces;
using ExpenseAI.Application.Queries.Budgets.DTOs;
using ExpenseAI.Domain.Entities;
using ExpenseAI.Domain.ValueObjects;
using CategoryVO = ExpenseAI.Domain.ValueObjects.Category;

namespace ExpenseAI.Application.Queries.Budgets.GetBudgetById;

public class GetBudgetByIdHandler : IQueryHandler<GetBudgetByIdQuery, BudgetDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBudgetByIdHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<BudgetDto?>> Handle(GetBudgetByIdQuery request, CancellationToken cancellationToken)
    {
        var budget = await _unitOfWork.Budgets.GetByIdAsync(request.Id, cancellationToken);

        if (budget == null)
            return Result.Success<BudgetDto?>(null);

        // Calculate spent amount and performance metrics
        var expenses = await _unitOfWork.Expenses.GetByUserIdAndDateRangeAsync(
            budget.UserId,
            budget.StartDate,
            budget.EndDate,
            cancellationToken);

        var categoryExpenses = expenses.Where(e => e.Category.Name == budget.Category).ToList();
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
            IsRecurring = budget.Period != BudgetPeriod.Monthly, // Simplified logic
            RecurrencePattern = budget.Period.ToString().ToLowerInvariant(),
            SpentAmount = spentAmount,
            RemainingAmount = remainingAmount,
            PercentageUsed = percentageUsed,
            IsOverBudget = spentAmount.Amount > budget.Amount.Amount,
            CreatedAt = budget.CreatedAt,
            UpdatedAt = budget.UpdatedAt
        };

        return Result.Success<BudgetDto?>(dto);
    }

    private static CategoryVO GetCategoryByName(string categoryName)
    {
        return CategoryVO.SystemCategories.FirstOrDefault(c =>
            c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase)) ?? CategoryVO.Uncategorized;
    }
}
