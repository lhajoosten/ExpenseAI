using FluentValidation;

namespace ExpenseAI.Application.Commands.Invoices.CreateInvoice;

public class CreateInvoiceValidator : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.ClientName)
            .NotEmpty()
            .WithMessage("Client name is required")
            .MaximumLength(200)
            .WithMessage("Client name must not exceed 200 characters");

        RuleFor(x => x.ClientEmail)
            .NotEmpty()
            .WithMessage("Client email is required")
            .EmailAddress()
            .WithMessage("Client email must be a valid email address")
            .MaximumLength(300)
            .WithMessage("Client email must not exceed 300 characters");

        RuleFor(x => x.ClientAddress)
            .MaximumLength(500)
            .WithMessage("Client address must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.ClientAddress));

        RuleFor(x => x.IssueDate)
            .NotEmpty()
            .WithMessage("Issue date is required");

        RuleFor(x => x.DueDate)
            .NotEmpty()
            .WithMessage("Due date is required")
            .GreaterThanOrEqualTo(x => x.IssueDate)
            .WithMessage("Due date must be on or after the issue date");

        RuleFor(x => x.TaxRate)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Tax rate must be greater than or equal to 0")
            .LessThanOrEqualTo(1)
            .WithMessage("Tax rate must be less than or equal to 1 (100%)");

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .WithMessage("Notes must not exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Notes));

        RuleFor(x => x.LineItems)
            .NotEmpty()
            .WithMessage("At least one line item is required")
            .Must(items => items.Count <= 50)
            .WithMessage("Cannot have more than 50 line items");

        RuleForEach(x => x.LineItems).SetValidator(new CreateInvoiceLineItemDtoValidator());
    }
}

public class CreateInvoiceLineItemDtoValidator : AbstractValidator<CreateInvoiceLineItemDto>
{
    public CreateInvoiceLineItemDtoValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Line item description is required")
            .MaximumLength(500)
            .WithMessage("Line item description must not exceed 500 characters");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Line item amount must be greater than 0")
            .LessThanOrEqualTo(1000000)
            .WithMessage("Line item amount must not exceed 1,000,000");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Currency is required")
            .Length(3)
            .WithMessage("Currency must be a 3-letter code (e.g., USD, EUR)")
            .Matches("^[A-Z]{3}$")
            .WithMessage("Currency must be uppercase letters only");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0")
            .LessThanOrEqualTo(10000)
            .WithMessage("Quantity must not exceed 10,000");
    }
}
