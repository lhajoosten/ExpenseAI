using ExpenseAI.Application.Common;
using ExpenseAI.Application.Queries.Budgets.DTOs;

namespace ExpenseAI.Application.Queries.Budgets.GetBudgetPerformance;

public record GetBudgetPerformanceQuery(
    Guid UserId,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate,
    Guid? CategoryId = null
) : IQuery<BudgetPerformanceDto>;
