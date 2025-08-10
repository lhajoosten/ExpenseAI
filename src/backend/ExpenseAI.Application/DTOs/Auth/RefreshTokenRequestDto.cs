namespace ExpenseAI.Application.DTOs.Auth;

/// <summary>
/// Request DTO for token refresh
/// </summary>
public class RefreshTokenRequestDto
{
    /// <summary>
    /// Refresh token
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
}
