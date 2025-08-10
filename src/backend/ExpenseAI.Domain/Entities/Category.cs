using ExpenseAI.Domain.Common;

namespace ExpenseAI.Domain.Entities;

/// <summary>
/// Category entity for organizing expenses
/// </summary>
public class Category : BaseEntity
{
    /// <summary>
    /// Category name
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Category description
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Category color (hex code)
    /// </summary>
    public string Color { get; private set; } = "#6B7280";

    /// <summary>
    /// Category icon name
    /// </summary>
    public string Icon { get; private set; } = "receipt";

    /// <summary>
    /// Whether this is a system-defined category
    /// </summary>
    public bool IsSystemCategory { get; private set; }

    /// <summary>
    /// Whether this category is active
    /// </summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// Category sort order for display
    /// </summary>
    public int SortOrder { get; private set; }

    /// <summary>
    /// EF Core constructor
    /// </summary>
    private Category() { }

    /// <summary>
    /// Create a new category
    /// </summary>
    public Category(
        string name, 
        string? description = null, 
        string color = "#6B7280", 
        string icon = "receipt", 
        bool isSystemCategory = false,
        int sortOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be null or empty", nameof(name));

        if (name.Length > 100)
            throw new ArgumentException("Category name cannot exceed 100 characters", nameof(name));

        Name = name.Trim();
        Description = description?.Trim();
        Color = color;
        Icon = icon;
        IsSystemCategory = isSystemCategory;
        SortOrder = sortOrder;
        IsActive = true;
    }

    /// <summary>
    /// Update category details
    /// </summary>
    public void Update(string name, string? description = null, string? color = null, string? icon = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be null or empty", nameof(name));

        if (name.Length > 100)
            throw new ArgumentException("Category name cannot exceed 100 characters", nameof(name));

        Name = name.Trim();
        Description = description?.Trim();
        
        if (!string.IsNullOrWhiteSpace(color))
            Color = color;
            
        if (!string.IsNullOrWhiteSpace(icon))
            Icon = icon;
    }

    /// <summary>
    /// Deactivate the category
    /// </summary>
    public void Deactivate()
    {
        if (IsSystemCategory)
            throw new InvalidOperationException("Cannot deactivate system categories");
            
        IsActive = false;
    }

    /// <summary>
    /// Activate the category
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Update sort order
    /// </summary>
    public void UpdateSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;
    }

    /// <summary>
    /// Create system categories
    /// </summary>
    public static List<Category> GetSystemCategories()
    {
        return new List<Category>
        {
            new Category("Food & Dining", "Restaurants, groceries, food delivery", "#FF5722", "restaurant", true, 1),
            new Category("Transportation", "Gas, public transport, ride sharing", "#2196F3", "directions_car", true, 2),
            new Category("Shopping", "Clothing, electronics, general purchases", "#FF9800", "shopping_cart", true, 3),
            new Category("Entertainment", "Movies, games, subscriptions", "#9C27B0", "movie", true, 4),
            new Category("Bills & Utilities", "Electricity, water, internet, phone", "#607D8B", "receipt", true, 5),
            new Category("Healthcare", "Medical expenses, pharmacy, insurance", "#4CAF50", "local_hospital", true, 6),
            new Category("Travel", "Hotels, flights, vacation expenses", "#00BCD4", "flight", true, 7),
            new Category("Education", "Books, courses, training", "#795548", "school", true, 8),
            new Category("Personal Care", "Haircuts, cosmetics, fitness", "#E91E63", "face", true, 9),
            new Category("Other", "Miscellaneous expenses", "#9E9E9E", "category", true, 10)
        };
    }
}
