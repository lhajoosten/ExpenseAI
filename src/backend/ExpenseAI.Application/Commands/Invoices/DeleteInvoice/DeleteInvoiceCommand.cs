using ExpenseAI.Application.Common;

namespace ExpenseAI.Application.Commands.Invoices.DeleteInvoice;

public record DeleteInvoiceCommand(
    Guid Id,
    Guid UserId
) : ICommand<bool>;
