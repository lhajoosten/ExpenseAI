using Ardalis.Result;
using ExpenseAI.Application.Common;
using ExpenseAI.Application.Interfaces;
using ExpenseAI.Application.Queries.Expenses.DTOs;

namespace ExpenseAI.Application.Queries.Expenses.GetExpenseStats;

public class GetExpenseStatsHandler : IQueryHandler<GetExpenseStatsQuery, ExpenseStatsDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetExpenseStatsHandler(IUnitOfWork unitOfWork)
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
