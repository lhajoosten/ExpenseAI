using ExpenseAI.Application.Common;

namespace ExpenseAI.Application.Commands.Invoices.UpdateInvoice;

public record UpdateInvoiceCommand(
    Guid Id,
    Guid UserId,
    string? CustomerName = null,
    string? CustomerEmail = null,
    string? CustomerAddress = null,
    string? Description = null,
    DateTimeOffset? DueDate = null,
    string? Notes = null,
    List<UpdateInvoiceLineItemDto>? LineItems = null
) : ICommand<bool>;

public record UpdateInvoiceLineItemDto(
    Guid? Id,
    string Description,
    int Quantity,
    decimal UnitPrice,
    string? Notes = null
);
