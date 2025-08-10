using ExpenseAI.Domain.ValueObjects;

namespace ExpenseAI.Application.Queries.Budgets.DTOs;

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

    // Additional properties for aggregate performance
    public string Period { get; init; } = string.Empty;
    public Money TotalBudgeted { get; init; } = Money.Zero("USD");
    public Money TotalSpent { get; init; } = Money.Zero("USD");
    public Money TotalRemaining { get; init; } = Money.Zero("USD");
    public IReadOnlyList<BudgetExpenseDto> BudgetExpenses { get; init; } = new List<BudgetExpenseDto>();
}
