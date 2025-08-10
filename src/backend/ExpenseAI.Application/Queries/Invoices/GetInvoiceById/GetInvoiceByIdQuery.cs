using ExpenseAI.Application.Common;
using ExpenseAI.Application.Queries.Invoices.DTOs;

namespace ExpenseAI.Application.Queries.Invoices.GetInvoiceById;

public record GetInvoiceByIdQuery(
    Guid Id,
    Guid UserId
) : IQuery<InvoiceDto?>;
