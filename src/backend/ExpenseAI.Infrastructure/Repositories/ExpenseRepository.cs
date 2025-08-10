using Microsoft.EntityFrameworkCore;
using ExpenseAI.Application.Interfaces;
using ExpenseAI.Domain.Entities;
using ExpenseAI.Domain.ValueObjects;
using ExpenseAI.Infrastructure.Data;
using CategoryVO = ExpenseAI.Domain.ValueObjects.Category;

namespace ExpenseAI.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for expense operations
/// </summary>
public class ExpenseRepository : BaseRepository<Expense>, IExpenseRepository
{
    public ExpenseRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Get expenses by user ID
    /// </summary>
    public async Task<IReadOnlyList<Expense>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.ExpenseDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get expenses by user ID with pagination
    /// </summary>
    public async Task<(IReadOnlyList<Expense> Items, int TotalCount)> GetByUserIdAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(e => e.UserId == userId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(e => e.ExpenseDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    /// <summary>
    /// Get expenses by date range
    /// </summary>
    public async Task<IReadOnlyList<Expense>> GetByDateRangeAsync(
        Guid userId,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => e.UserId == userId &&
                       e.ExpenseDate >= startDate &&
                       e.ExpenseDate <= endDate)
            .OrderByDescending(e => e.ExpenseDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get expenses by category
    /// </summary>
    public async Task<IReadOnlyList<Expense>> GetByCategoryAsync(
        Guid userId,
        CategoryVO category,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => e.UserId == userId && e.Category.Equals(category))
            .OrderByDescending(e => e.ExpenseDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get expenses by status
    /// </summary>
    public async Task<IReadOnlyList<Expense>> GetByStatusAsync(
        Guid userId,
        ExpenseStatus status,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => e.UserId == userId && e.Status == status)
            .OrderByDescending(e => e.ExpenseDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get total expenses by user
    /// </summary>
    public async Task<Money> GetTotalByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var expenses = await _dbSet
            .Where(e => e.UserId == userId && e.Status != ExpenseStatus.Rejected)
            .Select(e => e.Amount)
            .ToListAsync(cancellationToken);

        if (!expenses.Any())
            return Money.Zero("USD");

        // Group by currency and sum
        var groupedByCurrency = expenses
            .GroupBy(m => m.Currency)
            .Select(g => new Money(g.Sum(m => m.Amount), g.Key))
            .ToList();

        // For simplicity, return the first currency group or convert to USD
        return groupedByCurrency.FirstOrDefault() ?? Money.Zero("USD");
    }

    /// <summary>
    /// Get total expenses by user and date range
    /// </summary>
    public async Task<Money> GetTotalByDateRangeAsync(
        Guid userId,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        CancellationToken cancellationToken = default)
    {
        var expenses = await _dbSet
            .Where(e => e.UserId == userId &&
                       e.ExpenseDate >= startDate &&
                       e.ExpenseDate <= endDate &&
                       e.Status != ExpenseStatus.Rejected)
            .Select(e => e.Amount)
            .ToListAsync(cancellationToken);

        if (!expenses.Any())
            return Money.Zero("USD");

        // Group by currency and sum
        var groupedByCurrency = expenses
            .GroupBy(m => m.Currency)
            .Select(g => new Money(g.Sum(m => m.Amount), g.Key))
            .ToList();

        // For simplicity, return the first currency group or convert to USD
        return groupedByCurrency.FirstOrDefault() ?? Money.Zero("USD");
    }

    /// <summary>
    /// Get total expenses by user and date range (alias for compatibility)
    /// </summary>
    public async Task<Money> GetTotalByUserAndDateRangeAsync(
        Guid userId,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        CancellationToken cancellationToken = default)
    {
        return await GetTotalByDateRangeAsync(userId, startDate, endDate, cancellationToken);
    }

    /// <summary>
    /// Get expenses by user ID and date range (alias for compatibility)
    /// </summary>
    public async Task<IReadOnlyList<Expense>> GetByUserIdAndDateRangeAsync(
        Guid userId,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        CancellationToken cancellationToken = default)
    {
        return await GetByDateRangeAsync(userId, startDate, endDate, cancellationToken);
    }

    /// <summary>
    /// Advanced search with filtering and pagination
    /// </summary>
    public async Task<IReadOnlyList<Expense>> SearchAsync(
        Guid userId,
        string? searchTerm = null,
        string? category = null,
        decimal? minAmount = null,
        decimal? maxAmount = null,
        DateTimeOffset? startDate = null,
        DateTimeOffset? endDate = null,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(e => e.UserId == userId);

        // Apply search term filter
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLower();
            query = query.Where(e => e.Description.ToLower().Contains(lowerSearchTerm) ||
                                   (e.MerchantName != null && e.MerchantName.ToLower().Contains(lowerSearchTerm)));
        }

        // Apply category filter
        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(e => e.Category.Name.ToLower() == category.ToLower());
        }

        // Apply amount filters
        if (minAmount.HasValue)
        {
            query = query.Where(e => e.Amount.Amount >= minAmount.Value);
        }

        if (maxAmount.HasValue)
        {
            query = query.Where(e => e.Amount.Amount <= maxAmount.Value);
        }

        // Apply date range filters
        if (startDate.HasValue)
        {
            query = query.Where(e => e.ExpenseDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(e => e.ExpenseDate <= endDate.Value);
        }

        return await query
            .OrderByDescending(e => e.ExpenseDate)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get recent expenses by user
    /// </summary>
    public async Task<IReadOnlyList<Expense>> GetRecentAsync(
        Guid userId,
        int count = 10,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get expenses with receipts
    /// </summary>
    public async Task<IReadOnlyList<Expense>> GetWithReceiptsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => e.UserId == userId && e.ReceiptUrl != null)
            .OrderByDescending(e => e.ExpenseDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get pending reimbursements
    /// </summary>
    public async Task<IReadOnlyList<Expense>> GetPendingReimbursementsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => e.UserId == userId &&
                       e.IsReimbursable &&
                       !e.IsReimbursed &&
                       e.Status == ExpenseStatus.Approved)
            .OrderByDescending(e => e.ExpenseDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get expenses by date range
    /// </summary>
    public async Task<IReadOnlyList<Expense>> GetExpensesByDateRangeAsync(
        Guid userId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => e.UserId == userId &&
                       e.ExpenseDate >= startDate &&
                       e.ExpenseDate <= endDate)
            .OrderBy(e => e.ExpenseDate)
            .ToListAsync(cancellationToken);
    }
}
