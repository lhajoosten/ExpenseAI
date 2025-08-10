using Ardalis.Result;
using ExpenseAI.Application.Common;
using ExpenseAI.Application.Interfaces;
using ExpenseAI.Domain.Entities;

namespace ExpenseAI.Application.Queries.Expenses.GetExpenseById;

public class GetExpenseByIdHandler : IQueryHandler<GetExpenseByIdQuery, ExpenseDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetExpenseByIdHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ExpenseDto?>> Handle(GetExpenseByIdQuery request, CancellationToken cancellationToken)
    {
        var expense = await _unitOfWork.Expenses.GetByIdAsync(request.Id, cancellationToken);

        if (expense == null)
            return Result.Success<ExpenseDto?>(null);

        if (expense.UserId != request.UserId)
            return Result.Forbidden();

        var dto = MapToDto(expense);
        return Result.Success<ExpenseDto?>(dto);
    }

    private static ExpenseDto MapToDto(Expense expense)
    {
        return new ExpenseDto
        {
            Id = expense.Id,
            Description = expense.Description,
            Amount = expense.Amount.Amount,
            Currency = expense.Amount.Currency,
            CategoryName = expense.Category.Name,
            CategoryColor = expense.Category.Color,
            CategoryIcon = expense.Category.Icon,
            ExpenseDate = expense.ExpenseDate,
            Notes = expense.Notes,
            ReceiptUrl = expense.ReceiptUrl,
            MerchantName = expense.MerchantName,
            PaymentMethod = expense.PaymentMethod,
            IsReimbursable = expense.IsReimbursable,
            IsReimbursed = expense.IsReimbursed,
            Status = expense.Status.ToString(),
            IsAiCategorized = expense.IsAiCategorized,
            AiConfidenceScore = expense.AiConfidenceScore,
            Tags = expense.Tags.ToList(),
            CreatedAt = expense.CreatedAt,
            UpdatedAt = expense.UpdatedAt
        };
    }
}
