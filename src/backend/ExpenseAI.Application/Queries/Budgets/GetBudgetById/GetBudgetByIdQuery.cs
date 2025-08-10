using ExpenseAI.Application.Common;
using ExpenseAI.Application.Queries.Budgets.DTOs;

namespace ExpenseAI.Application.Queries.Budgets.GetBudgetById;

/// <summary>
/// Get budget by ID query
/// </summary>
public record GetBudgetByIdQuery(
    Guid Id
) : IQuery<BudgetDto?>;
