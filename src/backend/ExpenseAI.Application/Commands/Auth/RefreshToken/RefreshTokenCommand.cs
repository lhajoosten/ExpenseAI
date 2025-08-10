using ExpenseAI.Application.Common;

namespace ExpenseAI.Application.Commands.Auth.RefreshToken;

public record RefreshTokenCommand(
    string RefreshToken
) : ICommand<RefreshTokenResult>;
