using ExpenseAI.Domain.ValueObjects;

namespace ExpenseAI.Application.Queries.Invoices.DTOs;

public class InvoiceDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string ClientEmail { get; set; } = string.Empty;
    public string? ClientAddress { get; set; }
    public DateTimeOffset IssueDate { get; set; }
    public DateTimeOffset DueDate { get; set; }
    public Money SubtotalAmount { get; set; } = Money.Zero("USD");
    public Money TaxAmount { get; set; } = Money.Zero("USD");
    public Money TotalAmount { get; set; } = Money.Zero("USD");
    public double TaxRate { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset? PaidDate { get; set; }
    public string? PaymentMethod { get; set; }
    public string? PaymentReference { get; set; }
    public List<InvoiceLineItemDto> LineItems { get; set; } = new();
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
