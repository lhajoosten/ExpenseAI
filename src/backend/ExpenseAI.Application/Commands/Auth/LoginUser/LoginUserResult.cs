namespace ExpenseAI.Application.Commands.Auth.LoginUser;

public record LoginUserResult(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    bool EmailConfirmed
);
