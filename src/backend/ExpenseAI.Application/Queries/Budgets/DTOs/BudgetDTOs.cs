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

/// <summary>
/// Budget performance DTO
/// </summary>
public record BudgetPerformanceDto
{
    public Guid BudgetId { get; init; }
    public string BudgetName { get; init; } = string.Empty;
    public Money BudgetAmount { get; init; } = Money.Zero("USD");
    public Money SpentAmount { get; init; } = Money.Zero("USD");
    public Money RemainingAmount { get; init; } = Money.Zero("USD");
    public decimal PercentageUsed { get; init; }
    public bool IsOverBudget { get; init; }
    public int DaysRemaining { get; init; }
    public decimal AverageDailySpend { get; init; }
    public decimal ProjectedTotalSpend { get; init; }
    public IReadOnlyList<BudgetExpenseDto> RecentExpenses { get; init; } = new List<BudgetExpenseDto>();
}

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
}

/// <summary>
/// Paged result helper
/// </summary>
public record PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = new List<T>();
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}
