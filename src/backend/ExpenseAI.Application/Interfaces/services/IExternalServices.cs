using ExpenseAI.Domain.ValueObjects;

namespace ExpenseAI.Application.Interfaces;

public interface IAiExpenseCategorizationService
{
    Task<(Category category, double confidence)> CategorizeExpenseAsync(
        string description,
        string? merchantName = null,
        decimal? amount = null,
        string? receiptText = null,
        CancellationToken cancellationToken = default);
}

public interface IAiDocumentProcessingService
{
    Task<DocumentProcessingResult> ProcessReceiptAsync(
        Stream documentStream,
        string fileName,
        CancellationToken cancellationToken = default);

    Task<DocumentProcessingResult> ProcessInvoiceAsync(
        Stream documentStream,
        string fileName,
        CancellationToken cancellationToken = default);
}

public interface IAiNaturalLanguageQueryService
{
    Task<string> ProcessQueryAsync(
        string query,
        Guid userId,
        CancellationToken cancellationToken = default);
}

public interface IFileStorageService
{
    Task<string> UploadFileAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default);

    Task<Stream> DownloadFileAsync(
        string fileUrl,
        CancellationToken cancellationToken = default);

    Task DeleteFileAsync(
        string fileUrl,
        CancellationToken cancellationToken = default);
}

public interface IEmailService
{
    Task SendEmailAsync(
        string to,
        string subject,
        string body,
        bool isHtml = true,
        CancellationToken cancellationToken = default);

    Task SendInvoiceEmailAsync(
        string clientEmail,
        string clientName,
        string invoiceNumber,
        string pdfUrl,
        CancellationToken cancellationToken = default);

    // Auth-related email methods
    Task<Ardalis.Result.Result> SendEmailConfirmationAsync(string email, string token, CancellationToken cancellationToken = default);
    Task<Ardalis.Result.Result> SendPasswordResetEmailAsync(string email, string token, CancellationToken cancellationToken = default);
    Task<Ardalis.Result.Result> SendInvoiceAsync(Domain.Entities.Invoice invoice, string? customMessage = null, CancellationToken cancellationToken = default);
}

public class DocumentProcessingResult
{
    public bool IsSuccessful { get; set; }
    public string? ExtractedText { get; set; }
    public ExtractedExpenseData? ExpenseData { get; set; }
    public ExtractedInvoiceData? InvoiceData { get; set; }
    public List<string> Errors { get; set; } = new();
}

public class ExtractedExpenseData
{
    public string? MerchantName { get; set; }
    public decimal? Amount { get; set; }
    public string? Currency { get; set; }
    public DateTimeOffset? Date { get; set; }
    public string? Description { get; set; }
    public Category? SuggestedCategory { get; set; }
    public double CategoryConfidence { get; set; }
}

public class ExtractedInvoiceData
{
    public string? InvoiceNumber { get; set; }
    public string? ClientName { get; set; }
    public decimal? Amount { get; set; }
    public string? Currency { get; set; }
    public DateTimeOffset? Date { get; set; }
    public DateTimeOffset? DueDate { get; set; }
    public List<ExtractedLineItem> LineItems { get; set; } = new();
}

public class ExtractedLineItem
{
    public string Description { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; } = 1;
}
