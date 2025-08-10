using ExpenseAI.Domain.Common;
using ExpenseAI.Domain.ValueObjects;
using ExpenseAI.Domain.Events;
using CategoryVO = ExpenseAI.Domain.ValueObjects.Category;

namespace ExpenseAI.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string? ProfileImageUrl { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTimeOffset? LastLoginAt { get; private set; }
    public string PreferredCurrency { get; private set; } = "USD";
    public string TimeZone { get; private set; } = "UTC";

    // Navigation properties
    private readonly List<Expense> _expenses = new();
    public IReadOnlyList<Expense> Expenses => _expenses.AsReadOnly();

    private readonly List<CategoryVO> _customCategories = new();
    public IReadOnlyList<CategoryVO> CustomCategories => _customCategories.AsReadOnly();

    private User() { } // EF Core

    public User(string email, string firstName, string lastName, string? profileImageUrl = null)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty", nameof(email));

        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be null or empty", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be null or empty", nameof(lastName));

        Email = email.ToLowerInvariant();
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        ProfileImageUrl = profileImageUrl;

        AddDomainEvent(new UserCreatedEvent(Id, Email));
    }

    public string FullName => $"{FirstName} {LastName}";

    public void UpdateProfile(string firstName, string lastName, string? profileImageUrl = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be null or empty", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be null or empty", nameof(lastName));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        ProfileImageUrl = profileImageUrl;

        MarkAsUpdated();
    }

    public void UpdatePreferences(string preferredCurrency, string timeZone)
    {
        if (string.IsNullOrWhiteSpace(preferredCurrency))
            throw new ArgumentException("Preferred currency cannot be null or empty", nameof(preferredCurrency));

        if (string.IsNullOrWhiteSpace(timeZone))
            throw new ArgumentException("Time zone cannot be null or empty", nameof(timeZone));

        PreferredCurrency = preferredCurrency.ToUpperInvariant();
        TimeZone = timeZone;

        MarkAsUpdated();
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTimeOffset.UtcNow;
        MarkAsUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkAsUpdated();
        AddDomainEvent(new UserDeactivatedEvent(Id, Email));
    }

    public void Activate()
    {
        IsActive = true;
        MarkAsUpdated();
    }

    public void AddCustomCategory(CategoryVO category)
    {
        if (category.IsSystemCategory)
            throw new InvalidOperationException("Cannot add system categories as custom categories");

        if (_customCategories.Any(c => c.Name.Equals(category.Name, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"Category with name '{category.Name}' already exists");

        _customCategories.Add(category);
        MarkAsUpdated();
    }

    public IReadOnlyList<CategoryVO> GetAllAvailableCategories()
    {
        return CategoryVO.SystemCategories.Concat(_customCategories).ToList();
    }
}
