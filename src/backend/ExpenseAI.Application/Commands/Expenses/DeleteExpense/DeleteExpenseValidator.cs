using FluentValidation;

namespace ExpenseAI.Application.Commands.Expenses.DeleteExpense;

public class DeleteExpenseValidator : AbstractValidator<DeleteExpenseCommand>
{
    public DeleteExpenseValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Expense ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");
    }
}
