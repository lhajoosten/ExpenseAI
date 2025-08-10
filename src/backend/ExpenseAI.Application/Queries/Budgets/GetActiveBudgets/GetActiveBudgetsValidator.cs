using FluentValidation;

namespace ExpenseAI.Application.Queries.Budgets.GetActiveBudgets;

public class GetActiveBudgetsValidator : AbstractValidator<GetActiveBudgetsQuery>
{
    public GetActiveBudgetsValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required");
    }
}
