using FluentValidation;

namespace ExpenseAI.Application.Commands.Expenses.UploadReceipt;

public class UploadReceiptValidator : AbstractValidator<UploadReceiptCommand>
{
    private static readonly string[] AllowedFileTypes = { ".pdf", ".jpg", ".jpeg", ".png", ".gif" };
    private static readonly string[] AllowedContentTypes =
    {
        "application/pdf",
        "image/jpeg",
        "image/jpg",
        "image/png",
        "image/gif"
    };

    public UploadReceiptValidator()
    {
        RuleFor(x => x.ExpenseId)
            .NotEmpty()
            .WithMessage("Expense ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.FileStream)
            .NotNull()
            .WithMessage("File stream is required")
            .Must(stream => stream != null && stream.Length > 0)
            .WithMessage("File cannot be empty")
            .Must(stream => stream == null || stream.Length <= 10 * 1024 * 1024) // 10MB limit
            .WithMessage("File size cannot exceed 10MB");

        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage("File name is required")
            .Must(fileName => string.IsNullOrEmpty(fileName) || AllowedFileTypes.Any(ext =>
                fileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            .WithMessage("File type must be PDF, JPG, JPEG, PNG, or GIF");

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .WithMessage("Content type is required")
            .Must(contentType => string.IsNullOrEmpty(contentType) || AllowedContentTypes.Contains(contentType.ToLowerInvariant()))
            .WithMessage("Content type must be application/pdf, image/jpeg, image/png, or image/gif");
    }
}
