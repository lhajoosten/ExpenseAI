using ExpenseAI.Application.Common;
using ExpenseAI.Application.Queries.Budgets.DTOs;

namespace ExpenseAI.Application.Queries.Budgets.GetBudgetsByUser;

/// <summary>
/// Get budgets by user query
/// </summary>
public record GetBudgetsByUserQuery(
    Guid UserId,
    int Page = 1,
    int PageSize = 20
) : IQuery<PagedResult<BudgetDto>>;
