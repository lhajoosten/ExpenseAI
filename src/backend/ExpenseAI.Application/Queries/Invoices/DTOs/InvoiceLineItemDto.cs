using ExpenseAI.Domain.ValueObjects;

namespace ExpenseAI.Application.Queries.Invoices.DTOs;

public class InvoiceLineItemDto
{
    public string Description { get; set; } = string.Empty;
    public Money UnitPrice { get; set; } = Money.Zero("USD");
    public int Quantity { get; set; }
    public Money TotalPrice { get; set; } = Money.Zero("USD");
}
