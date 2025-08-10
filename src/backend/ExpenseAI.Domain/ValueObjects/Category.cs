using ExpenseAI.Domain.Common;

namespace ExpenseAI.Domain.ValueObjects;

public class Category : ValueObject
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string Color { get; private set; } = string.Empty;
    public string Icon { get; private set; } = string.Empty;
    public bool IsSystemCategory { get; private set; }

    private Category() { } // EF Core

    public Category(string name, string? description = null, string color = "#6B7280", string icon = "receipt", bool isSystemCategory = false)
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
    }

    public static Category Office => new("Office", "Office supplies and equipment", "#3B82F6", "building-office", true);
    public static Category Travel => new("Travel", "Business travel expenses", "#10B981", "airplane", true);
    public static Category Meals => new("Meals", "Business meals and entertainment", "#F59E0B", "utensils", true);
    public static Category Software => new("Software", "Software licenses and subscriptions", "#8B5CF6", "computer", true);
    public static Category Marketing => new("Marketing", "Marketing and advertising expenses", "#EF4444", "megaphone", true);
    public static Category Professional => new("Professional", "Professional services and consulting", "#6366F1", "briefcase", true);
    public static Category Uncategorized => new("Uncategorized", "Expenses that need categorization", "#9CA3AF", "question-mark", true);

    public static IReadOnlyList<Category> SystemCategories => new List<Category>
    {
        Office, Travel, Meals, Software, Marketing, Professional, Uncategorized
    };

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
        yield return Description ?? string.Empty;
        yield return Color;
        yield return Icon;
        yield return IsSystemCategory;
    }

    public override string ToString() => Name;
}
