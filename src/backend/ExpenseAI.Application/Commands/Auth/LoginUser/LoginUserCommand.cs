using ExpenseAI.Application.Common;

namespace ExpenseAI.Application.Commands.Auth.LoginUser;

public record LoginUserCommand(
    string Email,
    string Password,
    bool RememberMe = false
) : ICommand<LoginUserResult>;
