using ExpenseAI.Application.Common;
using ExpenseAI.Application.Queries.Expenses.DTOs;

namespace ExpenseAI.Application.Queries.Expenses.SearchExpenses;

public record SearchExpensesQuery(
    Guid UserId,
    string? SearchTerm = null,
    string? Category = null,
    decimal? MinAmount = null,
    decimal? MaxAmount = null,
    DateTimeOffset? StartDate = null,
    DateTimeOffset? EndDate = null,
    int Skip = 0,
    int Take = 50
) : IQuery<PagedResult<ExpenseDto>>;
