using ExpenseAI.Domain.ValueObjects;

namespace ExpenseAI.Domain.Entities;

/// <summary>
/// Budget entity for tracking spending limits
/// </summary>
public class Budget
{
    /// <summary>
    /// Unique identifier for the budget
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Budget name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Budget description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Budget amount with currency
    /// </summary>
    public Money Amount { get; set; } = Money.Zero("USD");

    /// <summary>
    /// Budget category
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Budget period type
    /// </summary>
    public BudgetPeriod Period { get; set; } = BudgetPeriod.Monthly;

    /// <summary>
    /// Budget start date
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Budget end date
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Alert threshold percentage (0-100)
    /// </summary>
    public decimal AlertThreshold { get; set; } = 80m;

    /// <summary>
    /// Indicates if the budget is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// User who owns this budget
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to the user
    /// </summary>
    public virtual ApplicationUser User { get; set; } = null!;

    /// <summary>
    /// Budget creation date
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Budget last update date
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Budget tags for categorization
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Budget color for UI display
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Budget icon for UI display
    /// </summary>
    public string? Icon { get; set; }
}

/// <summary>
/// Budget period enumeration
/// </summary>
public enum BudgetPeriod
{
    Weekly = 1,
    Monthly = 2,
    Quarterly = 3,
    Yearly = 4
}
