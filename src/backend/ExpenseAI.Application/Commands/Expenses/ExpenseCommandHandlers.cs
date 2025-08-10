using Ardalis.Result;
using ExpenseAI.Application.Common;
using ExpenseAI.Application.Interfaces;
using ExpenseAI.Domain.Entities;
using ExpenseAI.Domain.ValueObjects;
using CategoryVO = ExpenseAI.Domain.ValueObjects.Category;

namespace ExpenseAI.Application.Commands.Expenses;

public class CreateExpenseCommandHandler : ICommandHandler<CreateExpenseCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAiExpenseCategorizationService _aiService;

    public CreateExpenseCommandHandler(
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

public class UpdateExpenseCommandHandler : ICommandHandler<UpdateExpenseCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateExpenseCommandHandler(IUnitOfWork unitOfWork)
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

public class DeleteExpenseCommandHandler : ICommandHandler<DeleteExpenseCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteExpenseCommandHandler(IUnitOfWork unitOfWork)
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

public class UploadReceiptCommandHandler : ICommandHandler<UploadReceiptCommand, string>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;
    private readonly IAiDocumentProcessingService _aiDocumentService;

    public UploadReceiptCommandHandler(
        IUnitOfWork unitOfWork,
        IFileStorageService fileStorageService,
        IAiDocumentProcessingService aiDocumentService)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
        _aiDocumentService = aiDocumentService;
    }

    public async Task<Result<string>> Handle(UploadReceiptCommand request, CancellationToken cancellationToken)
    {
        var expense = await _unitOfWork.Expenses.GetByIdAsync(request.ExpenseId, cancellationToken);
        if (expense == null)
            return Result.NotFound("Expense not found");

        if (expense.UserId != request.UserId)
            return Result.Forbidden();

        try
        {
            // Upload file
            var fileUrl = await _fileStorageService.UploadFileAsync(
                request.FileStream,
                request.FileName,
                request.ContentType,
                cancellationToken);

            // Attach receipt to expense
            expense.AttachReceipt(fileUrl);

            // Process document with AI (optional enhancement)
            try
            {
                request.FileStream.Position = 0; // Reset stream
                var processingResult = await _aiDocumentService.ProcessReceiptAsync(
                    request.FileStream,
                    request.FileName,
                    cancellationToken);

                if (processingResult.IsSuccessful && processingResult.ExpenseData != null)
                {
                    expense.SetAiCategorization(
                        processingResult.ExpenseData.SuggestedCategory ?? CategoryVO.Uncategorized,
                        processingResult.ExpenseData.CategoryConfidence,
                        processingResult.ExtractedText);
                }
            }
            catch
            {
                // AI processing failed, continue without it
            }

            await _unitOfWork.Expenses.UpdateAsync(expense, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(fileUrl);
        }
        catch (Exception ex)
        {
            return Result.Error($"Failed to upload receipt: {ex.Message}");
        }
    }
}
