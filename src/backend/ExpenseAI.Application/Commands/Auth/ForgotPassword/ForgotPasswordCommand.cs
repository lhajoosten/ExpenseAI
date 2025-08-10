using ExpenseAI.Application.Common;

namespace ExpenseAI.Application.Commands.Auth.ForgotPassword;

public record ForgotPasswordCommand(
    string Email
) : ICommand<bool>;
