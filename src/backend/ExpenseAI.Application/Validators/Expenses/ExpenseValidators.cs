using FluentValidation;
using ExpenseAI.Application.Commands.Expenses;

namespace ExpenseAI.Application.Validators.Expenses;

public class CreateExpenseCommandValidator : AbstractValidator<CreateExpenseCommand>
{
    public CreateExpenseCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero")
            .LessThanOrEqualTo(999999.99m)
            .WithMessage("Amount cannot exceed 999,999.99");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Currency is required")
            .Length(3)
            .WithMessage("Currency must be a 3-letter ISO code")
            .Matches(@"^[A-Z]{3}$")
            .WithMessage("Currency must be uppercase letters only");

        RuleFor(x => x.CategoryName)
            .NotEmpty()
            .WithMessage("Category is required")
            .MaximumLength(100)
            .WithMessage("Category name cannot exceed 100 characters");

        RuleFor(x => x.ExpenseDate)
            .NotEmpty()
            .WithMessage("Expense date is required")
            .LessThanOrEqualTo(DateTimeOffset.UtcNow.AddDays(1))
            .WithMessage("Expense date cannot be in the future");

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .WithMessage("Notes cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Notes));

        RuleFor(x => x.MerchantName)
            .MaximumLength(200)
            .WithMessage("Merchant name cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.MerchantName));

        RuleFor(x => x.PaymentMethod)
            .MaximumLength(50)
            .WithMessage("Payment method cannot exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.PaymentMethod));
    }
}

public class UpdateExpenseCommandValidator : AbstractValidator<UpdateExpenseCommand>
{
    public UpdateExpenseCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Expense ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero")
            .LessThanOrEqualTo(999999.99m)
            .WithMessage("Amount cannot exceed 999,999.99");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Currency is required")
            .Length(3)
            .WithMessage("Currency must be a 3-letter ISO code")
            .Matches(@"^[A-Z]{3}$")
            .WithMessage("Currency must be uppercase letters only");

        RuleFor(x => x.CategoryName)
            .NotEmpty()
            .WithMessage("Category is required")
            .MaximumLength(100)
            .WithMessage("Category name cannot exceed 100 characters");

        RuleFor(x => x.ExpenseDate)
            .NotEmpty()
            .WithMessage("Expense date is required")
            .LessThanOrEqualTo(DateTimeOffset.UtcNow.AddDays(1))
            .WithMessage("Expense date cannot be in the future");

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .WithMessage("Notes cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Notes));

        RuleFor(x => x.MerchantName)
            .MaximumLength(200)
            .WithMessage("Merchant name cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.MerchantName));

        RuleFor(x => x.PaymentMethod)
            .MaximumLength(50)
            .WithMessage("Payment method cannot exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.PaymentMethod));
    }
}

public class DeleteExpenseCommandValidator : AbstractValidator<DeleteExpenseCommand>
{
    public DeleteExpenseCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Expense ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");
    }
}

public class UploadReceiptCommandValidator : AbstractValidator<UploadReceiptCommand>
{
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf" };
    private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10MB

    public UploadReceiptCommandValidator()
    {
        RuleFor(x => x.ExpenseId)
            .NotEmpty()
            .WithMessage("Expense ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage("File name is required")
            .Must(HaveValidExtension)
            .WithMessage($"File must have one of the following extensions: {string.Join(", ", AllowedExtensions)}");

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .WithMessage("Content type is required")
            .Must(BeValidContentType)
            .WithMessage("Invalid content type. Only images and PDFs are allowed.");

        RuleFor(x => x.FileStream)
            .NotNull()
            .WithMessage("File stream is required")
            .Must(HaveValidSize)
            .WithMessage($"File size cannot exceed {MaxFileSizeBytes / (1024 * 1024)}MB");
    }

    private static bool HaveValidExtension(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return false;

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return AllowedExtensions.Contains(extension);
    }

    private static bool BeValidContentType(string contentType)
    {
        if (string.IsNullOrEmpty(contentType))
            return false;

        var validContentTypes = new[]
        {
            "image/jpeg",
            "image/jpg",
            "image/png",
            "application/pdf"
        };

        return validContentTypes.Contains(contentType.ToLowerInvariant());
    }

    private static bool HaveValidSize(Stream fileStream)
    {
        return fileStream?.Length <= MaxFileSizeBytes;
    }
}
