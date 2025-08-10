namespace ExpenseAI.Application.Commands.Auth.RegisterUser;

public record RegisterUserResult(
    Guid UserId,
    string Email,
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt
);
