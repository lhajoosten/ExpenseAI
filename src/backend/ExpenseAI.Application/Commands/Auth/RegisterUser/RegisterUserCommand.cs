using ExpenseAI.Application.Common;

namespace ExpenseAI.Application.Commands.Auth.RegisterUser;

public record RegisterUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string? PhoneNumber = null
) : ICommand<RegisterUserResult>;
