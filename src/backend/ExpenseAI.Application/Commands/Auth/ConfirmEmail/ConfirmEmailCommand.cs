using ExpenseAI.Application.Common;

namespace ExpenseAI.Application.Commands.Auth.ConfirmEmail;

public record ConfirmEmailCommand(
    string Email,
    string Token
) : ICommand<bool>;
