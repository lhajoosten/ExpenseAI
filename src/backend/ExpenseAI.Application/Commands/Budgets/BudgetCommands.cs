using ExpenseAI.Application.Common;
using ExpenseAI.Domain.ValueObjects;
using CategoryVO = ExpenseAI.Domain.ValueObjects.Category;

namespace ExpenseAI.Application.Commands.Budgets;

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

/// <summary>
/// Update budget command
/// </summary>
public record UpdateBudgetCommand(
    Guid Id,
    Guid UserId,
    string Name,
    string Description,
    Money Amount,
    CategoryVO Category,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate,
    bool IsRecurring = false,
    string? RecurrencePattern = null
) : ICommand<bool>;

/// <summary>
/// Delete budget command
/// </summary>
public record DeleteBudgetCommand(
    Guid Id,
    Guid UserId
) : ICommand<bool>;

/// <summary>
/// Set budget alert command
/// </summary>
public record SetBudgetAlertCommand(
    Guid Id,
    Guid UserId,
    decimal ThresholdPercentage,
    bool IsEnabled
) : ICommand<bool>;
