using FluentValidation;

namespace ExpenseAI.Application.Commands.Invoices.DeleteInvoice;

public class DeleteInvoiceValidator : AbstractValidator<DeleteInvoiceCommand>
{
    public DeleteInvoiceValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Invoice ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required");
    }
}
