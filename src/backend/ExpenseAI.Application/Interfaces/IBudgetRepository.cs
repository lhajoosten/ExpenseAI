using ExpenseAI.Domain.Entities;
using ExpenseAI.Domain.ValueObjects;

namespace ExpenseAI.Application.Interfaces;

/// <summary>
/// Repository interface for budget operations
/// </summary>
public interface IBudgetRepository : IBaseRepository<Budget>
{
    /// <summary>
    /// Get budgets by user ID
    /// </summary>
    Task<IReadOnlyList<Budget>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get active budgets by user ID
    /// </summary>
    Task<IReadOnlyList<Budget>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get budgets by category
    /// </summary>
    Task<IReadOnlyList<Budget>> GetByCategoryAsync(
        Guid userId,
        string category,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get budgets by period
    /// </summary>
    Task<IReadOnlyList<Budget>> GetByPeriodAsync(
        Guid userId,
        BudgetPeriod period,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get budgets by date range
    /// </summary>
    Task<IReadOnlyList<Budget>> GetByDateRangeAsync(
        Guid userId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get current budgets (active within current period)
    /// </summary>
    Task<IReadOnlyList<Budget>> GetCurrentBudgetsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get budget by name
    /// </summary>
    Task<Budget?> GetByNameAsync(
        Guid userId,
        string name,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if budget name exists for user
    /// </summary>
    Task<bool> NameExistsAsync(
        Guid userId,
        string name,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get budgets exceeding alert threshold
    /// </summary>
    Task<IReadOnlyList<Budget>> GetBudgetsExceedingThresholdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get budget utilization (spent amount vs budget amount)
    /// </summary>
    Task<decimal> GetBudgetUtilizationAsync(
        Guid budgetId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get total budgeted amount by user
    /// </summary>
    Task<Money> GetTotalBudgetedAmountAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
