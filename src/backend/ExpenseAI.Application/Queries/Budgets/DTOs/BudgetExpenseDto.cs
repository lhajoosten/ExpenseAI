using ExpenseAI.Domain.ValueObjects;

namespace ExpenseAI.Application.Queries.Budgets.DTOs;

/// <summary>
/// Budget expense DTO for performance tracking
/// </summary>
public record BudgetExpenseDto
{
    public Guid Id { get; init; }
    public string Description { get; init; } = string.Empty;
    public Money Amount { get; init; } = Money.Zero("USD");
    public DateTimeOffset ExpenseDate { get; init; }
    public string? MerchantName { get; init; }
    public string CategoryName { get; init; } = string.Empty;
}
