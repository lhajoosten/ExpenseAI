using FluentValidation;

namespace ExpenseAI.Application.Queries.Budgets.GetBudgetById;

public class GetBudgetByIdValidator : AbstractValidator<GetBudgetByIdQuery>
{
    public GetBudgetByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Budget ID is required");
    }
}
