using Ardalis.Result;
using ExpenseAI.Application.Common;
using ExpenseAI.Application.Interfaces;
using ExpenseAI.Domain.Entities;

namespace ExpenseAI.Application.Commands.Invoices.DeleteInvoice;

public class DeleteInvoiceHandler : ICommandHandler<DeleteInvoiceCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteInvoiceHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _unitOfWork.Invoices.GetByIdAsync(request.Id, cancellationToken);
        if (invoice == null)
            return Result.NotFound("Invoice not found");

        if (invoice.UserId != request.UserId)
            return Result.Forbidden();

        // Check if invoice can be deleted (e.g., not already sent/paid)
        if (invoice.Status == InvoiceStatus.Paid)
            return Result.Error("Cannot delete a paid invoice");

        await _unitOfWork.Invoices.DeleteAsync(invoice, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}
