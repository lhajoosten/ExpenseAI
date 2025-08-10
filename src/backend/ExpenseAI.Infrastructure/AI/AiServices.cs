using Microsoft.Extensions.Options;
using ExpenseAI.Application.Interfaces;
using ExpenseAI.Domain.ValueObjects;

namespace ExpenseAI.Infrastructure.AI;

public class OpenAiExpenseCategorizationService : IAiExpenseCategorizationService
{
    private readonly HttpClient _httpClient;
    private readonly OpenAiSettings _settings;

    public OpenAiExpenseCategorizationService(HttpClient httpClient, IOptions<OpenAiSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
    }

    public async Task<(Category category, double confidence)> CategorizeExpenseAsync(
        string description,
        string? merchantName = null,
        decimal? amount = null,
        string? receiptText = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Create a comprehensive prompt for expense categorization
            var prompt = BuildCategorizationPrompt(description, merchantName, amount, receiptText);

            // Make API call to OpenAI
            var response = await CallOpenAiAsync(prompt, cancellationToken);

            // Parse response and determine category
            var (categoryName, confidence) = ParseCategorizationResponse(response);

            // Map to domain category
            var category = GetCategoryByName(categoryName);

            return (category, confidence);
        }
        catch (Exception)
        {
            // Fallback to Uncategorized with low confidence
            return (Category.Uncategorized, 0.0);
        }
    }

    private static string BuildCategorizationPrompt(string description, string? merchantName, decimal? amount, string? receiptText)
    {
        var systemCategories = string.Join(", ", Category.SystemCategories.Select(c => c.Name));

        var prompt = $@"
You are an AI assistant that categorizes business expenses.

Available categories: {systemCategories}

Expense Details:
- Description: {description}
- Merchant: {merchantName ?? "Unknown"}
- Amount: {amount?.ToString("C") ?? "Unknown"}
- Receipt Text: {receiptText ?? "None"}

Based on this information, determine the most appropriate category and your confidence level (0.0 to 1.0).

Respond in JSON format:
{{
    ""category"": ""CategoryName"",
    ""confidence"": 0.85
}}

Guidelines:
- Office: supplies, equipment, furniture, utilities
- Travel: flights, hotels, car rentals, gas, parking
- Meals: restaurants, catering, client entertainment
- Software: licenses, subscriptions, cloud services
- Marketing: advertising, promotional materials, trade shows
- Professional: consulting, legal, accounting services
- Use Uncategorized if unsure (confidence < 0.5)
";

        return prompt.Trim();
    }

    private async Task<string> CallOpenAiAsync(string prompt, CancellationToken cancellationToken)
    {
        // This is a simplified implementation
        // In a real implementation, you would:
        // 1. Set up proper authentication headers
        // 2. Create the proper OpenAI API request format
        // 3. Handle rate limiting and retries
        // 4. Parse the actual OpenAI response structure

        var request = new
        {
            model = _settings.Model,
            messages = new[]
            {
                new { role = "system", content = "You are a helpful AI assistant that categorizes business expenses." },
                new { role = "user", content = prompt }
            },
            max_tokens = 150,
            temperature = 0.1
        };

        // Placeholder response - in real implementation, this would be an actual API call
        // For now, we'll return a mock response
        await Task.Delay(100, cancellationToken); // Simulate API call delay

        return @"{
            ""category"": ""Office"",
            ""confidence"": 0.85
        }";
    }

    private static (string categoryName, double confidence) ParseCategorizationResponse(string response)
    {
        try
        {
            // In a real implementation, you would use a JSON parser
            // This is a simplified mock parsing
            if (response.Contains("Office"))
                return ("Office", 0.85);

            return ("Uncategorized", 0.3);
        }
        catch
        {
            return ("Uncategorized", 0.0);
        }
    }

    private static Category GetCategoryByName(string categoryName)
    {
        return Category.SystemCategories.FirstOrDefault(c =>
            c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase)) ?? Category.Uncategorized;
    }
}

public class AzureDocumentProcessingService : IAiDocumentProcessingService
{
    private readonly HttpClient _httpClient;
    private readonly AzureCognitiveSettings _settings;

    public AzureDocumentProcessingService(HttpClient httpClient, IOptions<AzureCognitiveSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
    }

    public async Task<DocumentProcessingResult> ProcessReceiptAsync(
        Stream documentStream,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // This is a placeholder implementation
            // In a real implementation, you would:
            // 1. Call Azure Form Recognizer or Document Intelligence
            // 2. Extract structured data from receipts
            // 3. Parse merchant names, amounts, dates, etc.

            await Task.Delay(1000, cancellationToken); // Simulate processing time

            return new DocumentProcessingResult
            {
                IsSuccessful = true,
                ExtractedText = "Mock extracted text from receipt",
                ExpenseData = new ExtractedExpenseData
                {
                    MerchantName = "Mock Merchant",
                    Amount = 45.67m,
                    Currency = "USD",
                    Date = DateTimeOffset.UtcNow.AddDays(-1),
                    Description = "Business supplies",
                    SuggestedCategory = Category.Office,
                    CategoryConfidence = 0.82
                }
            };
        }
        catch (Exception ex)
        {
            return new DocumentProcessingResult
            {
                IsSuccessful = false,
                Errors = new List<string> { $"Document processing failed: {ex.Message}" }
            };
        }
    }

    public async Task<DocumentProcessingResult> ProcessInvoiceAsync(
        Stream documentStream,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Placeholder implementation for invoice processing
            await Task.Delay(1500, cancellationToken);

            return new DocumentProcessingResult
            {
                IsSuccessful = true,
                ExtractedText = "Mock extracted text from invoice",
                InvoiceData = new ExtractedInvoiceData
                {
                    InvoiceNumber = "INV-001",
                    ClientName = "Mock Client",
                    Amount = 1250.00m,
                    Currency = "USD",
                    Date = DateTimeOffset.UtcNow,
                    DueDate = DateTimeOffset.UtcNow.AddDays(30),
                    LineItems = new List<ExtractedLineItem>
                    {
                        new() { Description = "Consulting Services", UnitPrice = 1250.00m, Quantity = 1 }
                    }
                }
            };
        }
        catch (Exception ex)
        {
            return new DocumentProcessingResult
            {
                IsSuccessful = false,
                Errors = new List<string> { $"Invoice processing failed: {ex.Message}" }
            };
        }
    }
}

public class OpenAiSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gpt-4-turbo";
    public string BaseUrl { get; set; } = "https://api.openai.com/v1";
}

public class AzureCognitiveSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string FormRecognizerModel { get; set; } = "prebuilt-receipt";
}
