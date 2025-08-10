using ExpenseAI.Application.Common;

namespace ExpenseAI.Application.Queries.Expenses.GetExpenseById;

public record GetExpenseByIdQuery(
    Guid Id,
    Guid UserId
) : IQuery<ExpenseDto?>;

public class ExpenseDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryColor { get; set; } = string.Empty;
    public string CategoryIcon { get; set; } = string.Empty;
    public DateTimeOffset ExpenseDate { get; set; }
    public string? Notes { get; set; }
    public string? ReceiptUrl { get; set; }
    public string? MerchantName { get; set; }
    public string? PaymentMethod { get; set; }
    public bool IsReimbursable { get; set; }
    public bool IsReimbursed { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsAiCategorized { get; set; }
    public double? AiConfidenceScore { get; set; }
    public List<string> Tags { get; set; } = new();
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
