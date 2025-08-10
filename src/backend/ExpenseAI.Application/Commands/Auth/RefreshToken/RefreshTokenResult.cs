namespace ExpenseAI.Application.Commands.Auth.RefreshToken;

public record RefreshTokenResult(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt
);
