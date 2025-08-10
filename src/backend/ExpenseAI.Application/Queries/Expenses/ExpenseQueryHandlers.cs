using Ardalis.Result;
using ExpenseAI.Application.Common;
using ExpenseAI.Application.Interfaces;
using ExpenseAI.Domain.Entities;

namespace ExpenseAI.Application.Queries.Expenses;

public class GetExpenseByIdQueryHandler : IQueryHandler<GetExpenseByIdQuery, ExpenseDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetExpenseByIdQueryHandler(IUnitOfWork unitOfWork)
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

public class GetExpensesByUserQueryHandler : IQueryHandler<GetExpensesByUserQuery, PagedResult<ExpenseDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetExpensesByUserQueryHandler(IUnitOfWork unitOfWork)
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

public class SearchExpensesQueryHandler : IQueryHandler<SearchExpensesQuery, PagedResult<ExpenseDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public SearchExpensesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PagedResult<ExpenseDto>>> Handle(SearchExpensesQuery request, CancellationToken cancellationToken)
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

        var result = new PagedResult<ExpenseDto>
        {
            Items = dtos,
            TotalCount = allExpenses.Count,
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

public class GetExpenseStatsQueryHandler : IQueryHandler<GetExpenseStatsQuery, ExpenseStatsDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetExpenseStatsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ExpenseStatsDto>> Handle(GetExpenseStatsQuery request, CancellationToken cancellationToken)
    {
        var expenses = await _unitOfWork.Expenses.GetByUserIdAndDateRangeAsync(
            request.UserId,
            request.StartDate ?? DateTimeOffset.MinValue,
            request.EndDate ?? DateTimeOffset.MaxValue,
            cancellationToken);

        if (!expenses.Any())
        {
            return Result.Success(new ExpenseStatsDto
            {
                Currency = "USD"
            });
        }

        var currency = expenses.First().Amount.Currency;
        var totalAmount = expenses.Sum(e => e.Amount.Amount);
        var averageAmount = totalAmount / expenses.Count;

        var categoryBreakdown = expenses
            .GroupBy(e => e.Category.Name)
            .Select(g => new CategoryStatsDto
            {
                CategoryName = g.Key,
                CategoryColor = g.First().Category.Color,
                TotalAmount = g.Sum(e => e.Amount.Amount),
                Count = g.Count(),
                Percentage = Math.Round((double)(g.Sum(e => e.Amount.Amount) / totalAmount) * 100, 2)
            })
            .OrderByDescending(c => c.TotalAmount)
            .ToList();

        var monthlyBreakdown = expenses
            .GroupBy(e => new { e.ExpenseDate.Year, e.ExpenseDate.Month })
            .Select(g => new MonthlyStatsDto
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                MonthName = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM"),
                TotalAmount = g.Sum(e => e.Amount.Amount),
                Count = g.Count()
            })
            .OrderBy(m => m.Year)
            .ThenBy(m => m.Month)
            .ToList();

        var pendingReimbursements = expenses
            .Where(e => e.IsReimbursable && !e.IsReimbursed)
            .Sum(e => e.Amount.Amount);

        var stats = new ExpenseStatsDto
        {
            TotalAmount = totalAmount,
            Currency = currency,
            TotalCount = expenses.Count,
            AverageAmount = averageAmount,
            CategoryBreakdown = categoryBreakdown,
            MonthlyBreakdown = monthlyBreakdown,
            PendingReimbursements = pendingReimbursements
        };

        return Result.Success(stats);
    }
}
