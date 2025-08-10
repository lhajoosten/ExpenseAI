using ExpenseAI.Application.Common;
using ExpenseAI.Domain.ValueObjects;
using CategoryVO = ExpenseAI.Domain.ValueObjects.Category;

namespace ExpenseAI.Application.Commands.Budgets.CreateBudget;

/// <summary>
/// Create budget command
/// </summary>
public record CreateBudgetCommand(
    Guid UserId,
    string Name,
    string Description,
    Money Amount,
    CategoryVO Category,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate,
    bool IsRecurring = false,
    string? RecurrencePattern = null
) : ICommand<Guid>;
