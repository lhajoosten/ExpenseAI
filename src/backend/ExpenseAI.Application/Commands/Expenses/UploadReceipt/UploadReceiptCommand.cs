using ExpenseAI.Application.Common;

namespace ExpenseAI.Application.Commands.Expenses.UploadReceipt;

public record UploadReceiptCommand(
    Guid ExpenseId,
    Guid UserId,
    Stream FileStream,
    string FileName,
    string ContentType
) : ICommand<string>;
