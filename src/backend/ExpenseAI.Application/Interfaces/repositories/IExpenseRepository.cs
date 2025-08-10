using ExpenseAI.Domain.Entities;
using ExpenseAI.Domain.ValueObjects;
using CategoryVO = ExpenseAI.Domain.ValueObjects.Category;

namespace ExpenseAI.Application.Interfaces;

/// <summary>
/// Repository interface for expense operations
/// </summary>
public interface IExpenseRepository : IBaseRepository<Expense>
{
    /// <summary>
    /// Get expenses by user ID
    /// </summary>
    Task<IReadOnlyList<Expense>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get expenses by user ID with pagination
    /// </summary>
    Task<(IReadOnlyList<Expense> Items, int TotalCount)> GetByUserIdAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get expenses by date range
    /// </summary>
    Task<IReadOnlyList<Expense>> GetByDateRangeAsync(
        Guid userId,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get expenses by user ID and date range (alias for compatibility)
    /// </summary>
    Task<IReadOnlyList<Expense>> GetByUserIdAndDateRangeAsync(
        Guid userId,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get expenses by category
    /// </summary>
    Task<IReadOnlyList<Expense>> GetByCategoryAsync(
        Guid userId,
        CategoryVO category,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get expenses by status
    /// </summary>
    Task<IReadOnlyList<Expense>> GetByStatusAsync(
        Guid userId,
        ExpenseStatus status,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get total expenses by user
    /// </summary>
    Task<Money> GetTotalByUserAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get total expenses by user and date range
    /// </summary>
    Task<Money> GetTotalByUserAndDateRangeAsync(
        Guid userId,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Advanced search expenses with filtering
    /// </summary>
    Task<IReadOnlyList<Expense>> SearchAsync(
        Guid userId,
        string? searchTerm = null,
        string? category = null,
        decimal? minAmount = null,
        decimal? maxAmount = null,
        DateTimeOffset? startDate = null,
        DateTimeOffset? endDate = null,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get recent expenses by user
    /// </summary>
    Task<IReadOnlyList<Expense>> GetRecentAsync(
        Guid userId,
        int count = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get expenses with receipts
    /// </summary>
    Task<IReadOnlyList<Expense>> GetWithReceiptsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get pending reimbursements
    /// </summary>
    Task<IReadOnlyList<Expense>> GetPendingReimbursementsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get expenses by date range
    /// </summary>
    Task<IReadOnlyList<Expense>> GetExpensesByDateRangeAsync(
        Guid userId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);
}
