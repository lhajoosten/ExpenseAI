using FluentValidation;

namespace ExpenseAI.Application.Queries.Budgets.GetBudgetsByUser;

public class GetBudgetsByUserValidator : AbstractValidator<GetBudgetsByUserQuery>
{
    public GetBudgetsByUserValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Page size must be greater than 0")
            .LessThanOrEqualTo(100)
            .WithMessage("Page size must not exceed 100");
    }
}
