using ExpenseAI.Application.Common;
using ExpenseAI.Application.Queries.Expenses.DTOs;

namespace ExpenseAI.Application.Queries.Expenses.GetExpenseStats;

public record GetExpenseStatsQuery(
    Guid UserId,
    DateTimeOffset? StartDate = null,
    DateTimeOffset? EndDate = null
) : IQuery<ExpenseStatsDto>;
