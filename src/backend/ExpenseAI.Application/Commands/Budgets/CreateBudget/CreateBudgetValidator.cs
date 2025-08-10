using FluentValidation;

namespace ExpenseAI.Application.Commands.Budgets.CreateBudget;

public class CreateBudgetValidator : AbstractValidator<CreateBudgetCommand>
{
    public CreateBudgetValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Budget name is required")
            .MaximumLength(100)
            .WithMessage("Budget name must not exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Amount)
            .NotNull()
            .WithMessage("Budget amount is required");

        RuleFor(x => x.Amount.Amount)
            .GreaterThan(0)
            .WithMessage("Budget amount must be greater than zero")
            .LessThanOrEqualTo(10000000)
            .WithMessage("Budget amount must not exceed 10,000,000")
            .When(x => x.Amount != null);

        RuleFor(x => x.Amount.Currency)
            .NotEmpty()
            .WithMessage("Currency is required")
            .Length(3)
            .WithMessage("Currency must be a 3-letter code (e.g., USD, EUR)")
            .Matches("^[A-Z]{3}$")
            .WithMessage("Currency must be uppercase letters only")
            .When(x => x.Amount != null);

        RuleFor(x => x.Category)
            .NotNull()
            .WithMessage("Category is required");

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("Start date is required")
            .GreaterThanOrEqualTo(DateTimeOffset.UtcNow.Date)
            .WithMessage("Start date cannot be in the past");

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .WithMessage("End date is required")
            .GreaterThan(x => x.StartDate)
            .WithMessage("End date must be after start date");

        RuleFor(x => x.RecurrencePattern)
            .MaximumLength(50)
            .WithMessage("Recurrence pattern must not exceed 50 characters")
            .Must(pattern => string.IsNullOrEmpty(pattern) || new[] { "weekly", "monthly", "quarterly", "yearly" }.Contains(pattern.ToLowerInvariant()))
            .WithMessage("Recurrence pattern must be one of: weekly, monthly, quarterly, yearly")
            .When(x => !string.IsNullOrEmpty(x.RecurrencePattern));
    }
}
