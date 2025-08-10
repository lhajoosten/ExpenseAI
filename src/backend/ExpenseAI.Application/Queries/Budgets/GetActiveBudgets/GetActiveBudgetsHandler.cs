using Ardalis.Result;
using ExpenseAI.Application.Common;
using ExpenseAI.Application.Interfaces;
using ExpenseAI.Application.Queries.Budgets.DTOs;
using ExpenseAI.Domain.Entities;
using ExpenseAI.Domain.ValueObjects;
using CategoryVO = ExpenseAI.Domain.ValueObjects.Category;

namespace ExpenseAI.Application.Queries.Budgets.GetActiveBudgets;

public class GetActiveBudgetsHandler : IQueryHandler<GetActiveBudgetsQuery, List<BudgetDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetActiveBudgetsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<BudgetDto>>> Handle(GetActiveBudgetsQuery request, CancellationToken cancellationToken)
    {
        var asOfDate = request.AsOfDate ?? DateTimeOffset.UtcNow;

        var budgets = await _unitOfWork.Budgets.GetActiveBudgetsAsync(cancellationToken);

        var dtos = budgets.Select(MapToDto).ToList();
        return Result.Success(dtos);
    }

    private static BudgetDto MapToDto(Budget budget)
    {
        return new BudgetDto
        {
            Id = budget.Id,
            UserId = budget.UserId,
            Name = budget.Name,
            Description = budget.Description ?? string.Empty,
            Amount = budget.Amount,
            Category = GetCategoryByName(budget.Category),
            StartDate = budget.StartDate,
            EndDate = budget.EndDate,
            IsRecurring = budget.IsRecurring,
            RecurrencePattern = budget.RecurrencePattern.ToString(),
            SpentAmount = new Money(budget.GetSpentAmount(), budget.Amount.Currency),
            RemainingAmount = new Money(budget.GetRemainingAmount(), budget.Amount.Currency),
            PercentageUsed = budget.GetPercentageUsed(),
            IsOverBudget = budget.IsOverBudget(),
            CreatedAt = budget.CreatedAt,
            UpdatedAt = budget.UpdatedAt
        };
    }

    private static CategoryVO GetCategoryByName(string categoryName)
    {
        return CategoryVO.SystemCategories.FirstOrDefault(c =>
            c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase)) ?? CategoryVO.Uncategorized;
    }
}
