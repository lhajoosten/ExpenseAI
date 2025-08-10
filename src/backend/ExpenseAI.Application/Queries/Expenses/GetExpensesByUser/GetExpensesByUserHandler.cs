using Ardalis.Result;
using ExpenseAI.Application.Common;
using ExpenseAI.Application.Interfaces;
using ExpenseAI.Application.Queries.Expenses.DTOs;
using ExpenseAI.Domain.Entities;

namespace ExpenseAI.Application.Queries.Expenses.GetExpensesByUser;

public class GetExpensesByUserHandler : IQueryHandler<GetExpensesByUserQuery, PagedResult<ExpenseDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetExpensesByUserHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PagedResult<ExpenseDto>>> Handle(GetExpensesByUserQuery request, CancellationToken cancellationToken)
    {
        var expenses = await _unitOfWork.Expenses.GetByUserIdAsync(request.UserId, cancellationToken);

        var orderedExpenses = expenses
            .OrderByDescending(e => e.ExpenseDate)
            .Skip(request.Skip)
            .Take(request.Take)
            .Select(MapToDto)
            .ToList();

        var result = new PagedResult<ExpenseDto>
        {
            Items = orderedExpenses,
            TotalCount = expenses.Count,
            PageSize = request.Take,
            PageNumber = (request.Skip / request.Take) + 1
        };

        return Result.Success(result);
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
