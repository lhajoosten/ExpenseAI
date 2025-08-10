using Ardalis.Result;
using ExpenseAI.Application.Common;
using ExpenseAI.Application.Interfaces;
using ExpenseAI.Application.Queries.Invoices.DTOs;
using ExpenseAI.Domain.Entities;

namespace ExpenseAI.Application.Queries.Invoices.GetInvoiceById;

public class GetInvoiceByIdHandler : IQueryHandler<GetInvoiceByIdQuery, InvoiceDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetInvoiceByIdHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<InvoiceDto?>> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        var invoice = await _unitOfWork.Invoices.GetByIdAsync(request.Id, cancellationToken);

        if (invoice == null)
            return Result.Success<InvoiceDto?>(null);

        if (invoice.UserId != request.UserId)
            return Result.Forbidden();

        var dto = MapToDto(invoice);
        return Result.Success<InvoiceDto?>(dto);
    }

    private static InvoiceDto MapToDto(Invoice invoice)
    {
        return new InvoiceDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            ClientName = invoice.ClientName,
            ClientEmail = invoice.ClientEmail,
            ClientAddress = invoice.ClientAddress,
            IssueDate = invoice.IssueDate,
            DueDate = invoice.DueDate,
            Status = invoice.Status.ToString(),
            SubtotalAmount = invoice.SubtotalAmount,
            TaxAmount = invoice.TaxAmount,
            TotalAmount = invoice.TotalAmount,
            TaxRate = invoice.TaxRate,
            Notes = invoice.Notes,
            PaidDate = invoice.PaidDate,
            PaymentMethod = invoice.PaymentMethod,
            PaymentReference = invoice.PaymentReference,
            LineItems = invoice.LineItems.Select(li => new InvoiceLineItemDto
            {
                Description = li.Description,
                Quantity = li.Quantity,
                UnitPrice = li.UnitPrice,
                TotalPrice = li.TotalPrice
            }).ToList(),
            CreatedAt = invoice.CreatedAt,
            UpdatedAt = invoice.UpdatedAt
        };
    }
}
