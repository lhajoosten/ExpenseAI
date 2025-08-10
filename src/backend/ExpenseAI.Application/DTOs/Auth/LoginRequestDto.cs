namespace ExpenseAI.Application.DTOs.Auth;

/// <summary>
/// Request DTO for user login
/// </summary>
public class LoginRequestDto
{
    /// <summary>
    /// User's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's password
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Remember user for extended session
    /// </summary>
    public bool RememberMe { get; set; } = false;
}
