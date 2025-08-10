using Ardalis.Result;
using ExpenseAI.Application.Common;
using ExpenseAI.Application.Interfaces;
using ExpenseAI.Application.Queries.Invoices.DTOs;
using ExpenseAI.Domain.Entities;

namespace ExpenseAI.Application.Queries.Invoices.GetInvoicesByUser;

public class GetInvoicesByUserHandler : IQueryHandler<GetInvoicesByUserQuery, Common.PagedResult<InvoiceSummaryDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetInvoicesByUserHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Common.PagedResult<InvoiceSummaryDto>>> Handle(GetInvoicesByUserQuery request, CancellationToken cancellationToken)
    {
        // Parse status filter
        InvoiceStatus? statusFilter = null;
        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<InvoiceStatus>(request.Status, true, out var parsedStatus))
        {
            statusFilter = parsedStatus;
        }

        // Use existing repository method with pagination
        var (invoices, totalCount) = await _unitOfWork.Invoices.GetByUserIdAsync(
            request.UserId,
            request.Page,
            request.PageSize,
            cancellationToken);

        // Apply additional filters if specified
        if (statusFilter.HasValue)
        {
            invoices = invoices.Where(i => i.Status == statusFilter.Value).ToList();
        }

        if (!string.IsNullOrWhiteSpace(request.CustomerName))
        {
            invoices = invoices.Where(i => i.ClientName.Contains(request.CustomerName, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (request.FromDate.HasValue)
        {
            invoices = invoices.Where(i => i.IssueDate >= request.FromDate.Value).ToList();
        }

        if (request.ToDate.HasValue)
        {
            invoices = invoices.Where(i => i.IssueDate <= request.ToDate.Value).ToList();
        }

        var dtos = invoices.Select(MapToSummaryDto).ToList();

        var result = new Common.PagedResult<InvoiceSummaryDto>
        {
            Items = dtos,
            TotalCount = dtos.Count, // Adjusted count after filtering
            Page = request.Page,
            PageSize = request.PageSize
        };

        return Result.Success(result);
    }    private static InvoiceSummaryDto MapToSummaryDto(Invoice invoice)
    {
        var today = DateTimeOffset.UtcNow;
        var daysUntilDue = (int)(invoice.DueDate - today).TotalDays;
        var isOverdue = invoice.DueDate < today && invoice.Status != InvoiceStatus.Paid;

        return new InvoiceSummaryDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            ClientName = invoice.ClientName,
            IssueDate = invoice.IssueDate,
            DueDate = invoice.DueDate,
            Status = invoice.Status.ToString(),
            TotalAmount = invoice.TotalAmount,
            IsOverdue = isOverdue,
            DaysUntilDue = daysUntilDue
        };
    }
}
