using ExpenseAI.Application.Common;
using ExpenseAI.Application.Queries.Expenses.DTOs;

namespace ExpenseAI.Application.Queries.Expenses.GetExpenseById;

public record GetExpenseByIdQuery(
    Guid Id,
    Guid UserId
) : IQuery<ExpenseDto?>;
