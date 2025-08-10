using FluentValidation;

namespace ExpenseAI.Application.Queries.Invoices.GetInvoiceById;

public class GetInvoiceByIdValidator : AbstractValidator<GetInvoiceByIdQuery>
{
    public GetInvoiceByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Invoice ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required");
    }
}
