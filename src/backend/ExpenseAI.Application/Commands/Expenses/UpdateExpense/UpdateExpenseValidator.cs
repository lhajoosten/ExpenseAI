using FluentValidation;

namespace ExpenseAI.Application.Commands.Expenses.UpdateExpense;

public class UpdateExpenseValidator : AbstractValidator<UpdateExpenseCommand>
{
    public UpdateExpenseValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Expense ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .MaximumLength(500)
            .WithMessage("Description must not exceed 500 characters");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero")
            .LessThanOrEqualTo(1000000)
            .WithMessage("Amount must not exceed 1,000,000");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Currency is required")
            .Length(3)
            .WithMessage("Currency must be a 3-letter code (e.g., USD, EUR)")
            .Matches("^[A-Z]{3}$")
            .WithMessage("Currency must be uppercase letters only");

        RuleFor(x => x.CategoryName)
            .NotEmpty()
            .WithMessage("Category name is required")
            .MaximumLength(100)
            .WithMessage("Category name must not exceed 100 characters");

        RuleFor(x => x.ExpenseDate)
            .NotEmpty()
            .WithMessage("Expense date is required")
            .LessThanOrEqualTo(DateTimeOffset.UtcNow.AddDays(1))
            .WithMessage("Expense date cannot be in the future");

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .WithMessage("Notes must not exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Notes));

        RuleFor(x => x.MerchantName)
            .MaximumLength(200)
            .WithMessage("Merchant name must not exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.MerchantName));

        RuleFor(x => x.PaymentMethod)
            .MaximumLength(50)
            .WithMessage("Payment method must not exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.PaymentMethod));
    }
}
