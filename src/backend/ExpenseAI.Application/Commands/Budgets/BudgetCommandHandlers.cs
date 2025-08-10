using ExpenseAI.Application.Commands.Budgets;
using ExpenseAI.Application.Common;
using ExpenseAI.Application.Interfaces;
using ExpenseAI.Domain.Entities;
using ExpenseAI.Domain.ValueObjects;
using Ardalis.Result;
using CategoryVO = ExpenseAI.Domain.ValueObjects.Category;

namespace ExpenseAI.Application.Commands.Budgets;

/// <summary>
/// Create budget command handler
/// </summary>
public class CreateBudgetCommandHandler : ICommandHandler<CreateBudgetCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateBudgetCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateBudgetCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Verify user exists
            var userExists = await _unitOfWork.Users.ExistsAsync(request.UserId, cancellationToken);
            if (!userExists)
            {
                return Result.Error("User not found");
            }

            // Validate date range
            if (request.StartDate >= request.EndDate)
            {
                return Result.Error("Start date must be before end date");
            }

            // Check for overlapping budgets in the same category
            var existingBudget = await _unitOfWork.Budgets.GetByUserAndCategoryAsync(
                request.UserId, request.Category.Name, cancellationToken);

            if (existingBudget != null)
            {
                var hasOverlap = request.StartDate < existingBudget.EndDate && request.EndDate > existingBudget.StartDate;
                if (hasOverlap)
                {
                    return Result.Error("A budget already exists for this category in the specified date range");
                }
            }

            // Create the budget
            var budget = Budget.Create(
                request.UserId,
                request.Name,
                request.Description ?? string.Empty,
                request.Amount,
                request.Category.Name,
                request.StartDate.DateTime,
                request.EndDate.DateTime,
                request.IsRecurring,
                ParseRecurrencePattern(request.RecurrencePattern));

            var createdBudget = await _unitOfWork.Budgets.AddAsync(budget, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(createdBudget.Id);
        }
        catch (Exception ex)
        {
            return Result.Error($"Failed to create budget: {ex.Message}");
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

/// <summary>
/// Update budget command handler
/// </summary>
public class UpdateBudgetCommandHandler : ICommandHandler<UpdateBudgetCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBudgetCommandHandler(IUnitOfWork unitOfWork)
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

/// <summary>
/// Delete budget command handler
/// </summary>
public class DeleteBudgetCommandHandler : ICommandHandler<DeleteBudgetCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBudgetCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteBudgetCommand request, CancellationToken cancellationToken)
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

            await _unitOfWork.Budgets.DeleteAsync(request.Id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            return Result.Error($"Failed to delete budget: {ex.Message}");
        }
    }
}

/// <summary>
/// Set budget alert command handler
/// </summary>
public class SetBudgetAlertCommandHandler : ICommandHandler<SetBudgetAlertCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetBudgetAlertCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(SetBudgetAlertCommand request, CancellationToken cancellationToken)
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

            if (request.IsEnabled)
            {
                budget.SetAlertThreshold(request.ThresholdPercentage);
            }
            else
            {
                budget.DisableAlerts();
            }

            await _unitOfWork.Budgets.UpdateAsync(budget, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            return Result.Error($"Failed to set budget alert: {ex.Message}");
        }
    }
}
