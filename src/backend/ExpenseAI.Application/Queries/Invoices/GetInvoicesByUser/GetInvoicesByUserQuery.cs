using ExpenseAI.Application.Common;
using ExpenseAI.Application.Queries.Invoices.DTOs;

namespace ExpenseAI.Application.Queries.Invoices.GetInvoicesByUser;

public record GetInvoicesByUserQuery(
    Guid UserId,
    int Page = 1,
    int PageSize = 10,
    string? Status = null,
    string? CustomerName = null,
    DateTimeOffset? FromDate = null,
    DateTimeOffset? ToDate = null
) : IQuery<Common.PagedResult<InvoiceSummaryDto>>;
