using ExpenseAI.Application.Common;

namespace ExpenseAI.Application.Commands.Expenses.CreateExpense;

public record CreateExpenseCommand(
    Guid UserId,
    string Description,
    decimal Amount,
    string Currency,
    string CategoryName,
    DateTimeOffset ExpenseDate,
    string? Notes = null,
    string? MerchantName = null,
    string? PaymentMethod = null,
    bool IsReimbursable = false
) : ICommand<Guid>;
