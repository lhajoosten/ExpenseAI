using ExpenseAI.Domain.Entities;

namespace ExpenseAI.Application.Interfaces;

/// <summary>
/// Repository interface for category operations
/// </summary>
public interface ICategoryRepository : IBaseRepository<Category>
{
    /// <summary>
    /// Get category by name
    /// </summary>
    Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all active categories
    /// </summary>
    Task<IReadOnlyList<Category>> GetActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get system categories
    /// </summary>
    Task<IReadOnlyList<Category>> GetSystemCategoriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get user-defined categories
    /// </summary>
    Task<IReadOnlyList<Category>> GetUserCategoriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get categories ordered by sort order
    /// </summary>
    Task<IReadOnlyList<Category>> GetOrderedAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if category name exists
    /// </summary>
    Task<bool> NameExistsAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get categories by color
    /// </summary>
    Task<IReadOnlyList<Category>> GetByColorAsync(string color, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update category sort orders
    /// </summary>
    Task UpdateSortOrdersAsync(Dictionary<Guid, int> sortOrders, CancellationToken cancellationToken = default);
}
