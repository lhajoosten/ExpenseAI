using Ardalis.Result;
using ExpenseAI.Application.Common;
using ExpenseAI.Application.Interfaces;
using ExpenseAI.Domain.ValueObjects;
using CategoryVO = ExpenseAI.Domain.ValueObjects.Category;

namespace ExpenseAI.Application.Commands.Expenses.UpdateExpense;

public class UpdateExpenseHandler : ICommandHandler<UpdateExpenseCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateExpenseHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
    {
        var expense = await _unitOfWork.Expenses.GetByIdAsync(request.Id, cancellationToken);
        if (expense == null)
            return Result.NotFound("Expense not found");

        if (expense.UserId != request.UserId)
            return Result.Forbidden();

        try
        {
            var amount = new Money(request.Amount, request.Currency);
            var category = GetCategoryByName(request.CategoryName);

            expense.UpdateDetails(
                request.Description,
                amount,
                category,
                request.ExpenseDate,
                request.Notes,
                request.MerchantName,
                request.PaymentMethod,
                request.IsReimbursable);

            await _unitOfWork.Expenses.UpdateAsync(expense, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (ArgumentException ex)
        {
            return Result.Invalid(new ValidationError { ErrorMessage = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Result.Invalid(new ValidationError { ErrorMessage = ex.Message });
        }
        catch (Exception ex)
        {
            return Result.Error($"Failed to update expense: {ex.Message}");
        }
    }

    private static CategoryVO GetCategoryByName(string categoryName)
    {
        return CategoryVO.SystemCategories.FirstOrDefault(c =>
            c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase)) ?? CategoryVO.Uncategorized;
    }
}
