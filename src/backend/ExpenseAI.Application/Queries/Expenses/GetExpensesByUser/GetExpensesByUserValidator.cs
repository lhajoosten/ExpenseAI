using FluentValidation;

namespace ExpenseAI.Application.Queries.Expenses.GetExpensesByUser;

public class GetExpensesByUserValidator : AbstractValidator<GetExpensesByUserQuery>
{
    public GetExpensesByUserValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

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
