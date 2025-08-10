using ExpenseAI.Application.Common;
using ExpenseAI.Domain.ValueObjects;

namespace ExpenseAI.Application.Commands.Expenses;

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

public record UpdateExpenseCommand(
    Guid Id,
    Guid UserId,
    string Description,
    decimal Amount,
    string Currency,
    string CategoryName,
    DateTimeOffset ExpenseDate,
    string? Notes = null,
    string? MerchantName = null,
    string? PaymentMethod = null,
    bool? IsReimbursable = null
) : ICommand;

public record DeleteExpenseCommand(
    Guid Id,
    Guid UserId
) : ICommand;

public record UploadReceiptCommand(
    Guid ExpenseId,
    Guid UserId,
    Stream FileStream,
    string FileName,
    string ContentType
) : ICommand<string>;

public record CategorizeExpenseWithAiCommand(
    Guid ExpenseId,
    Guid UserId
) : ICommand;
