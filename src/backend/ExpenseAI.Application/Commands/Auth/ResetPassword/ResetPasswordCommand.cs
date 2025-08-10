using ExpenseAI.Application.Common;

namespace ExpenseAI.Application.Commands.Auth.ResetPassword;

public record ResetPasswordCommand(
    string Email,
    string Token,
    string NewPassword
) : ICommand<bool>;
