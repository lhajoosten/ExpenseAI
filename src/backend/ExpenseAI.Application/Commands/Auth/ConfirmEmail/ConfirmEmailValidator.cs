using FluentValidation;

namespace ExpenseAI.Application.Commands.Auth.ConfirmEmail;

public class ConfirmEmailValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("A valid email address is required");

        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Confirmation token is required");
    }
}
