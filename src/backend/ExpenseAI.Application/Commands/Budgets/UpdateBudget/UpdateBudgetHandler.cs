using Ardalis.Result;
using ExpenseAI.Application.Common;
using ExpenseAI.Application.Interfaces;
using ExpenseAI.Domain.Entities;

namespace ExpenseAI.Application.Commands.Budgets.UpdateBudget;

/// <summary>
/// Update budget command handler
/// </summary>
public class UpdateBudgetHandler : ICommandHandler<UpdateBudgetCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBudgetHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(UpdateBudgetCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var budget = await _unitOfWork.Budgets.GetByIdAsync(request.Id, cancellationToken);
            if (budget == null)
            {
                return Result.Error("Budget not found");
            }

            if (budget.UserId != request.UserId.ToString())
            {
                return Result.Error("Unauthorized access to budget");
            }

            // Update budget properties
            budget.UpdateDetails(
                request.Name,
                request.Description ?? string.Empty,
                request.Amount);

            // Update category separately if needed
            budget.Category = request.Category.Name;

            budget.UpdateDateRange(request.StartDate.DateTime, request.EndDate.DateTime);

            if (request.IsRecurring)
            {
                budget.SetRecurrence(ParseRecurrencePattern(request.RecurrencePattern));
            }
            else
            {
                budget.RemoveRecurrence();
            }

            await _unitOfWork.Budgets.UpdateAsync(budget, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            return Result.Error($"Failed to update budget: {ex.Message}");
        }
    }

    private static BudgetPeriod ParseRecurrencePattern(string? pattern)
    {
        if (string.IsNullOrEmpty(pattern))
            return BudgetPeriod.Monthly;

        return pattern.ToLowerInvariant() switch
        {
            "weekly" => BudgetPeriod.Weekly,
            "monthly" => BudgetPeriod.Monthly,
            "quarterly" => BudgetPeriod.Quarterly,
            "yearly" => BudgetPeriod.Yearly,
            _ => BudgetPeriod.Monthly
        };
    }
}
