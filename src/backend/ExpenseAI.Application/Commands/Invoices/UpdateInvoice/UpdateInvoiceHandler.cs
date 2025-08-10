using Ardalis.Result;
using ExpenseAI.Application.Common;
using ExpenseAI.Application.Interfaces;
using ExpenseAI.Domain.Entities;
using ExpenseAI.Domain.ValueObjects;

namespace ExpenseAI.Application.Commands.Invoices.UpdateInvoice;

public class UpdateInvoiceHandler : ICommandHandler<UpdateInvoiceCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateInvoiceHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(UpdateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _unitOfWork.Invoices.GetByIdAsync(request.Id, cancellationToken);
        if (invoice == null)
            return Result.NotFound("Invoice not found");

        if (invoice.UserId != request.UserId)
            return Result.Forbidden();

        // Can only update draft invoices
        if (invoice.Status != InvoiceStatus.Draft)
            return Result.Error("Can only update draft invoices");

        // Update line items if provided
        if (request.LineItems != null)
        {
            // Remove existing line items
            while (invoice.LineItems.Count > 0)
            {
                invoice.RemoveLineItem(0);
            }

            // Add updated line items
            foreach (var lineItemDto in request.LineItems)
            {
                var money = new Money(lineItemDto.UnitPrice, "USD");
                invoice.AddLineItem(lineItemDto.Description, money, lineItemDto.Quantity);
            }
        }

        await _unitOfWork.Invoices.UpdateAsync(invoice, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}
