using ExpenseAI.Application.Common;
using ExpenseAI.Application.Queries.Expenses.DTOs;

namespace ExpenseAI.Application.Queries.Expenses.GetExpensesByUser;

public record GetExpensesByUserQuery(
    Guid UserId,
    int Skip = 0,
    int Take = 50
) : IQuery<PagedResult<ExpenseDto>>;
