using FluentValidation;

namespace ExpenseAI.Application.Queries.Budgets.GetBudgetPerformance;

public class GetBudgetPerformanceValidator : AbstractValidator<GetBudgetPerformanceQuery>
{
    public GetBudgetPerformanceValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required");

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("StartDate is required");

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .GreaterThan(x => x.StartDate)
            .WithMessage("EndDate must be after StartDate");
    }
}
