using FluentValidation;

namespace ExpenseAI.Application.Queries.Expenses.SearchExpenses;

public class SearchExpensesValidator : AbstractValidator<SearchExpensesQuery>
{
    public SearchExpensesValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.SearchTerm)
            .MaximumLength(200)
            .WithMessage("Search term must not exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.SearchTerm));

        RuleFor(x => x.Category)
            .MaximumLength(100)
            .WithMessage("Category must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Category));

        RuleFor(x => x.MinAmount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Minimum amount must be greater than or equal to 0")
            .When(x => x.MinAmount.HasValue);

        RuleFor(x => x.MaxAmount)
            .GreaterThan(0)
            .WithMessage("Maximum amount must be greater than 0")
            .When(x => x.MaxAmount.HasValue);

        RuleFor(x => x)
            .Must(x => x.MinAmount == null || x.MaxAmount == null || x.MinAmount <= x.MaxAmount)
            .WithMessage("Minimum amount must be less than or equal to maximum amount")
            .When(x => x.MinAmount.HasValue && x.MaxAmount.HasValue);

        RuleFor(x => x)
            .Must(x => x.StartDate == null || x.EndDate == null || x.StartDate <= x.EndDate)
            .WithMessage("Start date must be less than or equal to end date")
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue);

        RuleFor(x => x.Skip)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Skip must be greater than or equal to 0");

        RuleFor(x => x.Take)
            .GreaterThan(0)
            .WithMessage("Take must be greater than 0")
            .LessThanOrEqualTo(100)
            .WithMessage("Take must not exceed 100");
    }
}
