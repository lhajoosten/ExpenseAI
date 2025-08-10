using Ardalis.Result;
using ExpenseAI.Application.Common;
using ExpenseAI.Application.Interfaces;
using ExpenseAI.Application.Queries.Budgets.DTOs;
using ExpenseAI.Domain.ValueObjects;

namespace ExpenseAI.Application.Queries.Budgets.GetBudgetPerformance;

public class GetBudgetPerformanceHandler : IQueryHandler<GetBudgetPerformanceQuery, BudgetPerformanceDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBudgetPerformanceHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<BudgetPerformanceDto>> Handle(GetBudgetPerformanceQuery request, CancellationToken cancellationToken)
    {
        var budgets = await _unitOfWork.Budgets.GetBudgetsByDateRangeAsync(
            request.UserId,
            request.StartDate.DateTime,
            request.EndDate.DateTime,
            cancellationToken);

        if (request.CategoryId.HasValue)
        {
            // TODO: Fix category filtering - budget.Category is string, need to convert
            // budgets = budgets.Where(b => b.Category == categoryName).ToList();
        }

        var expenses = await _unitOfWork.Expenses.GetExpensesByDateRangeAsync(
            request.UserId,
            request.StartDate.DateTime,
            request.EndDate.DateTime,
            cancellationToken);

        if (request.CategoryId.HasValue)
        {
            // TODO: Fix category filtering - need to map CategoryId to name
            // expenses = expenses.Where(e => e.Category.Name == categoryName).ToList();
        }

        var totalBudgeted = budgets.Sum(b => b.Amount.Amount);
        var totalSpent = expenses.Sum(e => e.Amount.Amount);
        var totalRemaining = totalBudgeted - totalSpent;
        var percentageUsed = totalBudgeted > 0 ? (totalSpent / totalBudgeted) * 100 : 0;

        var budgetExpenses = budgets.Select(budget =>
        {
            var budgetExpensesList = expenses
                .Where(e => e.Category.Name == budget.Category &&
                           e.ExpenseDate >= budget.StartDate &&
                           e.ExpenseDate <= budget.EndDate)
                .Select(e => new BudgetExpenseDto
                {
                    Id = e.Id,
                    Description = e.Description,
                    Amount = e.Amount,
                    ExpenseDate = e.ExpenseDate,
                    CategoryName = e.Category.Name
                })
                .ToList();

            return new { Budget = budget, Expenses = budgetExpensesList };
        }).ToList();

        var performance = new BudgetPerformanceDto
        {
            Period = $"{request.StartDate:yyyy-MM-dd} to {request.EndDate:yyyy-MM-dd}",
            TotalBudgeted = new Money(totalBudgeted, budgets.FirstOrDefault()?.Amount.Currency ?? "USD"),
            TotalSpent = new Money(totalSpent, budgets.FirstOrDefault()?.Amount.Currency ?? "USD"),
            TotalRemaining = new Money(totalRemaining, budgets.FirstOrDefault()?.Amount.Currency ?? "USD"),
            PercentageUsed = (decimal)percentageUsed,
            IsOverBudget = totalSpent > totalBudgeted,
            BudgetExpenses = budgetExpenses.SelectMany(be => be.Expenses).ToList()
        };

        return Result.Success(performance);
    }
}
