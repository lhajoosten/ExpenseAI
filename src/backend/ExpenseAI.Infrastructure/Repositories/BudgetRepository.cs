using Microsoft.EntityFrameworkCore;
using ExpenseAI.Application.Interfaces;
using ExpenseAI.Domain.Entities;
using ExpenseAI.Domain.ValueObjects;
using ExpenseAI.Infrastructure.Data;

namespace ExpenseAI.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for budget operations
/// </summary>
public class BudgetRepository : BaseRepository<Budget>, IBudgetRepository
{
    public BudgetRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Get budgets by user ID
    /// </summary>
    public async Task<IReadOnlyList<Budget>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userIdString = userId.ToString();
        return await _dbSet
            .Where(b => b.UserId == userIdString)
            .OrderBy(b => b.StartDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get active budgets by user ID
    /// </summary>
    public async Task<IReadOnlyList<Budget>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userIdString = userId.ToString();
        return await _dbSet
            .Where(b => b.UserId == userIdString && b.IsActive)
            .OrderBy(b => b.StartDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get budgets by category
    /// </summary>
    public async Task<IReadOnlyList<Budget>> GetByCategoryAsync(
        Guid userId,
        string category,
        CancellationToken cancellationToken = default)
    {
        var userIdString = userId.ToString();
        return await _dbSet
            .Where(b => b.UserId == userIdString && b.Category == category)
            .OrderBy(b => b.StartDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get budgets by period
    /// </summary>
    public async Task<IReadOnlyList<Budget>> GetByPeriodAsync(
        Guid userId,
        BudgetPeriod period,
        CancellationToken cancellationToken = default)
    {
        var userIdString = userId.ToString();
        return await _dbSet
            .Where(b => b.UserId == userIdString && b.Period == period)
            .OrderBy(b => b.StartDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get budgets by date range
    /// </summary>
    public async Task<IReadOnlyList<Budget>> GetByDateRangeAsync(
        Guid userId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        var userIdString = userId.ToString();
        return await _dbSet
            .Where(b => b.UserId == userIdString &&
                       b.StartDate <= endDate &&
                       b.EndDate >= startDate)
            .OrderBy(b => b.StartDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get current budgets (active within current period)
    /// </summary>
    public async Task<IReadOnlyList<Budget>> GetCurrentBudgetsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var userIdString = userId.ToString();
        var currentDate = DateTime.UtcNow;

        return await _dbSet
            .Where(b => b.UserId == userIdString &&
                       b.IsActive &&
                       b.StartDate <= currentDate &&
                       b.EndDate >= currentDate)
            .OrderBy(b => b.StartDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get budget by name
    /// </summary>
    public async Task<Budget?> GetByNameAsync(
        Guid userId,
        string name,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        var userIdString = userId.ToString();
        return await _dbSet
            .FirstOrDefaultAsync(b => b.UserId == userIdString && b.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    /// <summary>
    /// Check if budget name exists for user
    /// </summary>
    public async Task<bool> NameExistsAsync(
        Guid userId,
        string name,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        var userIdString = userId.ToString();
        var query = _dbSet.Where(b => b.UserId == userIdString && b.Name.ToLower() == name.ToLower());

        if (excludeId.HasValue)
        {
            query = query.Where(b => b.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Get budgets exceeding alert threshold
    /// </summary>
    public async Task<IReadOnlyList<Budget>> GetBudgetsExceedingThresholdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var userIdString = userId.ToString();
        // This would require joining with expenses to calculate spent amounts
        // For now, return budgets that have alert thresholds set
        return await _dbSet
            .Where(b => b.UserId == userIdString &&
                       b.IsActive &&
                       b.AlertThreshold > 0)
            .OrderBy(b => b.StartDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get budget utilization (spent amount vs budget amount)
    /// </summary>
    public async Task<decimal> GetBudgetUtilizationAsync(
        Guid budgetId,
        CancellationToken cancellationToken = default)
    {
        // This would require joining with expenses to calculate spent amounts
        // For now, return 0 as a placeholder
        var budget = await GetByIdAsync(budgetId, cancellationToken);
        if (budget == null)
            return 0;

        // TODO: Calculate actual utilization by joining with expenses
        return 0;
    }

    /// <summary>
    /// Get total budgeted amount by user
    /// </summary>
    public async Task<Money> GetTotalBudgetedAmountAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var userIdString = userId.ToString();
        var budgets = await _dbSet
            .Where(b => b.UserId == userIdString && b.IsActive)
            .Select(b => b.Amount)
            .ToListAsync(cancellationToken);

        if (!budgets.Any())
            return Money.Zero("USD");

        // Group by currency and sum
        var groupedByCurrency = budgets
            .GroupBy(m => m.Currency)
            .Select(g => new Money(g.Sum(m => m.Amount), g.Key))
            .ToList();

        // For simplicity, return the first currency group or convert to USD
        return groupedByCurrency.FirstOrDefault() ?? Money.Zero("USD");
    }
}
