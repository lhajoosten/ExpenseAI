using FluentValidation;

namespace ExpenseAI.Application.Commands.Invoices.UpdateInvoice;

public class UpdateInvoiceValidator : AbstractValidator<UpdateInvoiceCommand>
{
    public UpdateInvoiceValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Invoice ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required");

        When(x => x.CustomerName != null, () =>
        {
            RuleFor(x => x.CustomerName)
                .NotEmpty()
                .MaximumLength(200)
                .WithMessage("Customer name must be between 1 and 200 characters");
        });

        When(x => x.CustomerEmail != null, () =>
        {
            RuleFor(x => x.CustomerEmail)
                .EmailAddress()
                .MaximumLength(320)
                .WithMessage("Customer email must be a valid email address");
        });

        When(x => x.CustomerAddress != null, () =>
        {
            RuleFor(x => x.CustomerAddress)
                .NotEmpty()
                .MaximumLength(500)
                .WithMessage("Customer address must be between 1 and 500 characters");
        });

        When(x => x.Description != null, () =>
        {
            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(1000)
                .WithMessage("Description must be between 1 and 1000 characters");
        });

        When(x => x.LineItems != null, () =>
        {
            RuleFor(x => x.LineItems)
                .NotEmpty()
                .WithMessage("At least one line item is required");

            RuleForEach(x => x.LineItems).SetValidator(new UpdateInvoiceLineItemValidator());
        });
    }
}

public class UpdateInvoiceLineItemValidator : AbstractValidator<UpdateInvoiceLineItemDto>
{
    public UpdateInvoiceLineItemValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(500)
            .WithMessage("Line item description is required and must not exceed 500 characters");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0)
            .WithMessage("Unit price must be greater than 0");

        When(x => x.Notes != null, () =>
        {
            RuleFor(x => x.Notes)
                .MaximumLength(1000)
                .WithMessage("Notes must not exceed 1000 characters");
        });
    }
}
