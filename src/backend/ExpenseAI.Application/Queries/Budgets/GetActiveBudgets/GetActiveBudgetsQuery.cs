using ExpenseAI.Application.Common;
using ExpenseAI.Application.Queries.Budgets.DTOs;

namespace ExpenseAI.Application.Queries.Budgets.GetActiveBudgets;

public record GetActiveBudgetsQuery(
    Guid UserId,
    DateTimeOffset? AsOfDate = null
) : IQuery<List<BudgetDto>>;
