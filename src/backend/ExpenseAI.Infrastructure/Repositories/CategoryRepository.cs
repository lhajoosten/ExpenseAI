using Microsoft.EntityFrameworkCore;
using ExpenseAI.Application.Interfaces;
using ExpenseAI.Domain.Entities;
using ExpenseAI.Infrastructure.Data;
using CategoryEntity = ExpenseAI.Domain.Entities.Category;

namespace ExpenseAI.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for category operations
/// </summary>
public class CategoryRepository : BaseRepository<CategoryEntity>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Get category by name
    /// </summary>
    public async Task<CategoryEntity?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        return await _dbSet
            .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    /// <summary>
    /// Get all active categories
    /// </summary>
    public async Task<IReadOnlyList<CategoryEntity>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.IsActive)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get system categories
    /// </summary>
    public async Task<IReadOnlyList<CategoryEntity>> GetSystemCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.IsSystemCategory)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get user-defined categories
    /// </summary>
    public async Task<IReadOnlyList<CategoryEntity>> GetUserCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => !c.IsSystemCategory && c.IsActive)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get categories ordered by sort order
    /// </summary>
    public async Task<IReadOnlyList<CategoryEntity>> GetOrderedAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.IsActive)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Check if category name exists
    /// </summary>
    public async Task<bool> NameExistsAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        var query = _dbSet.Where(c => c.Name.ToLower() == name.ToLower());

        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Get categories by color
    /// </summary>
    public async Task<IReadOnlyList<CategoryEntity>> GetByColorAsync(string color, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(color))
            return new List<CategoryEntity>();

        return await _dbSet
            .Where(c => c.Color.ToLower() == color.ToLower() && c.IsActive)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Update category sort orders
    /// </summary>
    public async Task UpdateSortOrdersAsync(Dictionary<Guid, int> sortOrders, CancellationToken cancellationToken = default)
    {
        if (!sortOrders.Any())
            return;

        var categoryIds = sortOrders.Keys.ToList();
        var categories = await _dbSet
            .Where(c => categoryIds.Contains(c.Id))
            .ToListAsync(cancellationToken);

        foreach (var category in categories)
        {
            if (sortOrders.TryGetValue(category.Id, out var sortOrder))
            {
                category.UpdateSortOrder(sortOrder);
            }
        }
    }

    // Implementation note: We need to cast CategoryEntity to Category for the interface
    // This is a temporary solution until we resolve the Category value object vs entity issue

    Task<Category?> ICategoryRepository.GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        // This is a placeholder - we need to resolve the Category value object vs entity confusion
        throw new NotImplementedException("Category entity/value object mapping needs to be resolved");
    }

    Task<IReadOnlyList<Category>> ICategoryRepository.GetActiveAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException("Category entity/value object mapping needs to be resolved");
    }

    Task<IReadOnlyList<Category>> ICategoryRepository.GetSystemCategoriesAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException("Category entity/value object mapping needs to be resolved");
    }

    Task<IReadOnlyList<Category>> ICategoryRepository.GetUserCategoriesAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException("Category entity/value object mapping needs to be resolved");
    }

    Task<IReadOnlyList<Category>> ICategoryRepository.GetOrderedAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException("Category entity/value object mapping needs to be resolved");
    }

    Task<IReadOnlyList<Category>> ICategoryRepository.GetByColorAsync(string color, CancellationToken cancellationToken)
    {
        throw new NotImplementedException("Category entity/value object mapping needs to be resolved");
    }
}
