using Ardalis.Result;
using ExpenseAI.Application.Common;
using ExpenseAI.Application.Interfaces;
using ExpenseAI.Domain.Entities;
using ExpenseAI.Domain.ValueObjects;

namespace ExpenseAI.Application.Commands.Invoices.CreateInvoice;

public class CreateInvoiceHandler : ICommandHandler<CreateInvoiceCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateInvoiceHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Verify user exists
            var userExists = await _unitOfWork.Users.ExistsAsync(request.UserId, cancellationToken);
            if (!userExists)
            {
                return Result.Error("User not found");
            }

            // Create the invoice
            var invoice = new Invoice(
                request.UserId,
                request.ClientName,
                request.ClientEmail,
                request.IssueDate,
                request.DueDate,
                request.TaxRate,
                request.ClientAddress,
                request.Notes);

            // Add line items
            foreach (var lineItemDto in request.LineItems)
            {
                var unitPrice = new Money(lineItemDto.Amount, lineItemDto.Currency);
                invoice.AddLineItem(lineItemDto.Description, unitPrice, lineItemDto.Quantity);
            }

            var createdInvoice = await _unitOfWork.Invoices.AddAsync(invoice, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(createdInvoice.Id);
        }
        catch (ArgumentException ex)
        {
            return Result.Invalid(new ValidationError { ErrorMessage = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Result.Invalid(new ValidationError { ErrorMessage = ex.Message });
        }
        catch (Exception ex)
        {
            return Result.Error($"Failed to create invoice: {ex.Message}");
        }
    }
}
