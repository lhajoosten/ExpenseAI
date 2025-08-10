using Ardalis.Result;
using ExpenseAI.Application.Common;
using ExpenseAI.Application.Interfaces;
using ExpenseAI.Domain.Entities;

namespace ExpenseAI.Application.Commands.Budgets.CreateBudget;

/// <summary>
/// Create budget command handler
/// </summary>
public class CreateBudgetHandler : ICommandHandler<CreateBudgetCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateBudgetHandler(IUnitOfWork unitOfWork)
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
