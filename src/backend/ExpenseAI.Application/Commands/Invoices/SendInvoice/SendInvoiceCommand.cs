using ExpenseAI.Application.Common;

namespace ExpenseAI.Application.Commands.Invoices.SendInvoice;

public record SendInvoiceCommand(
    Guid InvoiceId,
    Guid UserId,
    string? CustomMessage = null
) : ICommand<bool>;
