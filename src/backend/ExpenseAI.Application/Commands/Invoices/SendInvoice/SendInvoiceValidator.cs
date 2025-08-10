using FluentValidation;

namespace ExpenseAI.Application.Commands.Invoices.SendInvoice;

public class SendInvoiceValidator : AbstractValidator<SendInvoiceCommand>
{
    public SendInvoiceValidator()
    {
        RuleFor(x => x.InvoiceId)
            .NotEmpty()
            .WithMessage("Invoice ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required");

        When(x => x.CustomMessage != null, () =>
        {
            RuleFor(x => x.CustomMessage)
                .MaximumLength(2000)
                .WithMessage("Custom message must not exceed 2000 characters");
        });
    }
}
