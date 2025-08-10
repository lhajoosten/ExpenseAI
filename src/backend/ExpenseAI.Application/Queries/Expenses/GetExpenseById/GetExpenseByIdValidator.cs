using FluentValidation;

namespace ExpenseAI.Application.Queries.Expenses.GetExpenseById;

public class GetExpenseByIdValidator : AbstractValidator<GetExpenseByIdQuery>
{
    public GetExpenseByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Expense ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");
    }
}
