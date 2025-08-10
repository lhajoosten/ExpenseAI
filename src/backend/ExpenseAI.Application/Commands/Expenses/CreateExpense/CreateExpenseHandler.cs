using Ardalis.Result;
using ExpenseAI.Application.Common;
using ExpenseAI.Application.Interfaces;
using ExpenseAI.Domain.Entities;
using ExpenseAI.Domain.ValueObjects;
using CategoryVO = ExpenseAI.Domain.ValueObjects.Category;

namespace ExpenseAI.Application.Commands.Expenses.CreateExpense;

public class CreateExpenseHandler : ICommandHandler<CreateExpenseCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAiExpenseCategorizationService _aiService;

    public CreateExpenseHandler(
        IUnitOfWork unitOfWork,
        IAiExpenseCategorizationService aiService)
    {
        _unitOfWork = unitOfWork;
        _aiService = aiService;
    }

    public async Task<Result<Guid>> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
    {
        // Validate user exists
        var userExists = await _unitOfWork.Users.ExistsAsync(request.UserId, cancellationToken);
        if (!userExists)
            return Result.NotFound("User not found");

        try
        {
            // Create money value object
            var amount = new Money(request.Amount, request.Currency);

            // Get or create category
            var category = GetCategoryByName(request.CategoryName);

            // Create expense
            var expense = new Expense(
                request.UserId,
                request.Description,
                amount,
                category,
                request.ExpenseDate,
                request.Notes,
                request.MerchantName,
                request.PaymentMethod,
                request.IsReimbursable);

            // Try AI categorization if using uncategorized
            if (category == CategoryVO.Uncategorized)
            {
                try
                {
                    var (aiCategory, confidence) = await _aiService.CategorizeExpenseAsync(
                        request.Description,
                        request.MerchantName,
                        request.Amount,
                        cancellationToken: cancellationToken);

                    if (confidence > 0.7) // High confidence threshold
                    {
                        expense.SetAiCategorization(aiCategory, confidence);
                    }
                }
                catch
                {
                    // AI categorization failed, continue with manual category
                }
            }

            var createdExpense = await _unitOfWork.Expenses.AddAsync(expense, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(createdExpense.Id);
        }
        catch (ArgumentException ex)
        {
            return Result.Invalid(new ValidationError { ErrorMessage = ex.Message });
        }
        catch (Exception ex)
        {
            return Result.Error($"Failed to create expense: {ex.Message}");
        }
    }

    private static CategoryVO GetCategoryByName(string categoryName)
    {
        return CategoryVO.SystemCategories.FirstOrDefault(c =>
            c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase)) ?? CategoryVO.Uncategorized;
    }
}
