using Ardalis.Result;
using ExpenseAI.Application.Common;
using ExpenseAI.Application.Interfaces;
using ExpenseAI.Domain.ValueObjects;
using CategoryVO = ExpenseAI.Domain.ValueObjects.Category;

namespace ExpenseAI.Application.Commands.Expenses.UploadReceipt;

public class UploadReceiptHandler : ICommandHandler<UploadReceiptCommand, string>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;
    private readonly IAiDocumentProcessingService _aiDocumentService;

    public UploadReceiptHandler(
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
