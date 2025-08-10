using Ardalis.Result;
using ExpenseAI.Application.Common;
using ExpenseAI.Application.Interfaces;

namespace ExpenseAI.Application.Commands.Budgets.DeleteBudget;

/// <summary>
/// Delete budget command handler
/// </summary>
public class DeleteBudgetHandler : ICommandHandler<DeleteBudgetCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBudgetHandler(IUnitOfWork unitOfWork)
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
