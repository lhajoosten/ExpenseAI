using ExpenseAI.Application.Common;

namespace ExpenseAI.Application.Commands.Budgets.DeleteBudget;

/// <summary>
/// Delete budget command
/// </summary>
public record DeleteBudgetCommand(
    Guid Id,
    Guid UserId
) : ICommand<bool>;
