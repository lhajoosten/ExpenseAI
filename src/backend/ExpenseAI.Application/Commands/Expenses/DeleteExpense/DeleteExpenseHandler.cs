using Ardalis.Result;
using ExpenseAI.Application.Common;
using ExpenseAI.Application.Interfaces;

namespace ExpenseAI.Application.Commands.Expenses.DeleteExpense;

public class DeleteExpenseHandler : ICommandHandler<DeleteExpenseCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteExpenseHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
    {
        var expense = await _unitOfWork.Expenses.GetByIdAsync(request.Id, cancellationToken);
        if (expense == null)
            return Result.NotFound("Expense not found");

        if (expense.UserId != request.UserId)
            return Result.Forbidden();

        try
        {
            await _unitOfWork.Expenses.DeleteAsync(request.Id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Error($"Failed to delete expense: {ex.Message}");
        }
    }
}
