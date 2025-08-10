using ExpenseAI.Application.Common;

namespace ExpenseAI.Application.Commands.Invoices.CreateInvoice;

public record CreateInvoiceCommand(
    Guid UserId,
    string ClientName,
    string ClientEmail,
    string? ClientAddress,
    DateTimeOffset IssueDate,
    DateTimeOffset DueDate,
    double TaxRate,
    string? Notes,
    List<CreateInvoiceLineItemDto> LineItems
) : ICommand<Guid>;

public record CreateInvoiceLineItemDto(
    string Description,
    decimal Amount,
    string Currency,
    int Quantity = 1
);
