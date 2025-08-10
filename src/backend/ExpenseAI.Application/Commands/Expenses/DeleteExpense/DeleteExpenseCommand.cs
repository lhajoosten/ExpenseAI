using ExpenseAI.Application.Common;

namespace ExpenseAI.Application.Commands.Expenses.DeleteExpense;

public record DeleteExpenseCommand(
    Guid Id,
    Guid UserId
) : ICommand;
