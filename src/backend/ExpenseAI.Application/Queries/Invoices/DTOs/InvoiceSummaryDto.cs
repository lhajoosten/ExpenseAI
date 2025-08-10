using ExpenseAI.Domain.ValueObjects;

namespace ExpenseAI.Application.Queries.Invoices.DTOs;

public class InvoiceSummaryDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public DateTimeOffset IssueDate { get; set; }
    public DateTimeOffset DueDate { get; set; }
    public Money TotalAmount { get; set; } = Money.Zero("USD");
    public string Status { get; set; } = string.Empty;
    public bool IsOverdue { get; set; }
    public int DaysUntilDue { get; set; }
}
