using ExpenseAI.Domain.ValueObjects;
using CategoryVO = ExpenseAI.Domain.ValueObjects.Category;

namespace ExpenseAI.Application.Queries.Budgets.DTOs;

/// <summary>
/// Budget DTO for responses
/// </summary>
public record BudgetDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public Money Amount { get; init; } = Money.Zero("USD");
    public CategoryVO Category { get; init; } = CategoryVO.Uncategorized;
    public DateTimeOffset StartDate { get; init; }
    public DateTimeOffset EndDate { get; init; }
    public bool IsRecurring { get; init; }
    public string? RecurrencePattern { get; init; }
    public Money SpentAmount { get; init; } = Money.Zero("USD");
    public Money RemainingAmount { get; init; } = Money.Zero("USD");
    public decimal PercentageUsed { get; init; }
    public bool IsOverBudget { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }
}
