using Ardalis.Result;
using ExpenseAI.Application.Common;
using ExpenseAI.Application.Interfaces;
using ExpenseAI.Application.Queries.Expenses.DTOs;
using ExpenseAI.Domain.Entities;

namespace ExpenseAI.Application.Queries.Expenses.SearchExpenses;

public class SearchExpensesHandler : IQueryHandler<SearchExpensesQuery, Common.PagedResult<ExpenseDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public SearchExpensesHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Common.PagedResult<ExpenseDto>>> Handle(SearchExpensesQuery request, CancellationToken cancellationToken)
    {
        var expenses = await _unitOfWork.Expenses.SearchAsync(
            request.UserId,
            request.SearchTerm,
            request.Category,
            request.MinAmount,
            request.MaxAmount,
            request.StartDate,
            request.EndDate,
            request.Skip,
            request.Take,
            cancellationToken);

        var dtos = expenses.Select(MapToDto).ToList();

        // Get total count for pagination (simplified - in real implementation, repository should return count)
        var allExpenses = await _unitOfWork.Expenses.SearchAsync(
            request.UserId,
            request.SearchTerm,
            request.Category,
            request.MinAmount,
            request.MaxAmount,
            request.StartDate,
            request.EndDate,
            0,
            int.MaxValue,
            cancellationToken);

        var result = new Common.PagedResult<ExpenseDto>
        {
            Items = dtos,
            TotalCount = allExpenses.Count,
            PageSize = request.Take,
            Page = (request.Skip / request.Take) + 1
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
