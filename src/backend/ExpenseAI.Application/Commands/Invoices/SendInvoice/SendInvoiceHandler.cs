using Ardalis.Result;
using ExpenseAI.Application.Common;
using ExpenseAI.Application.Interfaces;
using ExpenseAI.Domain.Entities;
using static ExpenseAI.Domain.Entities.InvoiceStatus;

namespace ExpenseAI.Application.Commands.Invoices.SendInvoice;

public class SendInvoiceHandler : ICommandHandler<SendInvoiceCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;

    public SendInvoiceHandler(IUnitOfWork unitOfWork, IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
    }

    public async Task<Result<bool>> Handle(SendInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _unitOfWork.Invoices.GetByIdAsync(request.InvoiceId, cancellationToken);
        if (invoice == null)
            return Result.NotFound("Invoice not found");

        if (invoice.UserId != request.UserId)
            return Result.Forbidden();

                if (string.IsNullOrEmpty(invoice.ClientEmail))
            return Result.Invalid(new ValidationError("Customer email is required to send invoice"));

        if (invoice.Status == Paid)
            return Result.Invalid(new ValidationError("Cannot send a paid invoice"));

        // Send the invoice via email
        var emailResult = await _emailService.SendInvoiceAsync(
            invoice,
            request.CustomMessage,
            cancellationToken);

        if (!emailResult.IsSuccess)
            return Result.Error("Failed to send invoice email");

        // Update invoice status to sent
        invoice.Send();        await _unitOfWork.Invoices.UpdateAsync(invoice, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}
