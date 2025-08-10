using Microsoft.AspNetCore.Identity;

namespace ExpenseAI.Domain.Entities;

/// <summary>
/// Identity user entity for authentication and authorization
/// </summary>
public class ExpenseAIIdentityUser : IdentityUser<Guid>
{
    /// <summary>
    /// User's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// User's full name
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// URL to user's profile picture
    /// </summary>
    public string? ProfilePictureUrl { get; set; }

    /// <summary>
    /// User's date of birth
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// User's timezone
    /// </summary>
    public string TimeZone { get; set; } = "UTC";

    /// <summary>
    /// User's preferred currency
    /// </summary>
    public string PreferredCurrency { get; set; } = "USD";

    /// <summary>
    /// User's refresh token for JWT authentication
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Refresh token expiry time
    /// </summary>
    public DateTime? RefreshTokenExpiryTime { get; set; }

    /// <summary>
    /// Indicates if the user is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Date when the user was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date when the user was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date when the user last logged in
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Navigation property for user's expenses
    /// </summary>
    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();

    /// <summary>
    /// Navigation property for user's invoices
    /// </summary>
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    /// <summary>
    /// Navigation property for user's budgets
    /// </summary>
    public virtual ICollection<Budget> Budgets { get; set; } = new List<Budget>();
}
