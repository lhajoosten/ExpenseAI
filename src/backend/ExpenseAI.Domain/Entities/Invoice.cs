using ExpenseAI.Domain.Common;
using ExpenseAI.Domain.ValueObjects;
using ExpenseAI.Domain.Events;

namespace ExpenseAI.Domain.Entities;

public class Invoice : BaseEntity
{
    public string InvoiceNumber { get; private set; } = string.Empty;
    public string ClientName { get; private set; } = string.Empty;
    public string ClientEmail { get; private set; } = string.Empty;
    public string? ClientAddress { get; private set; }
    public DateTimeOffset IssueDate { get; private set; }
    public DateTimeOffset DueDate { get; private set; }
    public Money SubtotalAmount { get; private set; } = Money.Zero("USD");
    public Money TaxAmount { get; private set; } = Money.Zero("USD");
    public Money TotalAmount { get; private set; } = Money.Zero("USD");
    public double TaxRate { get; private set; }
    public string? Notes { get; private set; }
    public InvoiceStatus Status { get; private set; } = InvoiceStatus.Draft;
    public DateTimeOffset? PaidDate { get; private set; }
    public string? PaymentMethod { get; private set; }
    public string? PaymentReference { get; private set; }

    // Foreign keys
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    // Line items
    private readonly List<InvoiceLineItem> _lineItems = new();
    public IReadOnlyList<InvoiceLineItem> LineItems => _lineItems.AsReadOnly();

    private Invoice() { } // EF Core

    public Invoice(
        Guid userId,
        string clientName,
        string clientEmail,
        DateTimeOffset issueDate,
        DateTimeOffset dueDate,
        double taxRate = 0.0,
        string? clientAddress = null,
        string? notes = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        if (string.IsNullOrWhiteSpace(clientName))
            throw new ArgumentException("Client name cannot be null or empty", nameof(clientName));

        if (string.IsNullOrWhiteSpace(clientEmail))
            throw new ArgumentException("Client email cannot be null or empty", nameof(clientEmail));

        if (dueDate < issueDate)
            throw new ArgumentException("Due date cannot be before issue date", nameof(dueDate));

        if (taxRate < 0 || taxRate > 1)
            throw new ArgumentException("Tax rate must be between 0 and 1", nameof(taxRate));

        UserId = userId;
        InvoiceNumber = GenerateInvoiceNumber();
        ClientName = clientName.Trim();
        ClientEmail = clientEmail.Trim().ToLowerInvariant();
        ClientAddress = clientAddress?.Trim();
        IssueDate = issueDate;
        DueDate = dueDate;
        TaxRate = taxRate;
        Notes = notes?.Trim();
    }

    public void AddLineItem(string description, Money unitPrice, int quantity = 1)
    {
        if (Status == InvoiceStatus.Sent || Status == InvoiceStatus.Paid)
            throw new InvalidOperationException("Cannot modify sent or paid invoices");

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be null or empty", nameof(description));

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        var lineItem = new InvoiceLineItem(description, unitPrice, quantity);
        _lineItems.Add(lineItem);

        RecalculateAmounts();
        MarkAsUpdated();
    }

    public void RemoveLineItem(int index)
    {
        if (Status == InvoiceStatus.Sent || Status == InvoiceStatus.Paid)
            throw new InvalidOperationException("Cannot modify sent or paid invoices");

        if (index < 0 || index >= _lineItems.Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        _lineItems.RemoveAt(index);
        RecalculateAmounts();
        MarkAsUpdated();
    }

    public void UpdateLineItem(int index, string description, Money unitPrice, int quantity)
    {
        if (Status == InvoiceStatus.Sent || Status == InvoiceStatus.Paid)
            throw new InvalidOperationException("Cannot modify sent or paid invoices");

        if (index < 0 || index >= _lineItems.Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        _lineItems[index].Update(description, unitPrice, quantity);
        RecalculateAmounts();
        MarkAsUpdated();
    }

    public void Send()
    {
        if (Status != InvoiceStatus.Draft)
            throw new InvalidOperationException($"Cannot send invoice in {Status} status");

        if (!_lineItems.Any())
            throw new InvalidOperationException("Cannot send invoice without line items");

        Status = InvoiceStatus.Sent;
        MarkAsUpdated();

        AddDomainEvent(new InvoiceGeneratedEvent(Id, UserId, TotalAmount.Amount, TotalAmount.Currency));
    }

    public void MarkAsPaid(DateTimeOffset paidDate, string? paymentMethod = null, string? paymentReference = null)
    {
        if (Status != InvoiceStatus.Sent)
            throw new InvalidOperationException($"Cannot mark invoice as paid in {Status} status");

        Status = InvoiceStatus.Paid;
        PaidDate = paidDate;
        PaymentMethod = paymentMethod?.Trim();
        PaymentReference = paymentReference?.Trim();
        MarkAsUpdated();
    }

    public void Cancel()
    {
        if (Status == InvoiceStatus.Paid)
            throw new InvalidOperationException("Cannot cancel paid invoice");

        Status = InvoiceStatus.Cancelled;
        MarkAsUpdated();
    }

    private void RecalculateAmounts()
    {
        if (!_lineItems.Any())
        {
            SubtotalAmount = Money.Zero(SubtotalAmount.Currency);
            TaxAmount = Money.Zero(SubtotalAmount.Currency);
            TotalAmount = Money.Zero(SubtotalAmount.Currency);
            return;
        }

        var currency = _lineItems.First().TotalPrice.Currency;
        var subtotal = _lineItems
            .Select(item => item.TotalPrice)
            .Aggregate((a, b) => a.Add(b));

        SubtotalAmount = subtotal;
        TaxAmount = subtotal.Multiply((decimal)TaxRate);
        TotalAmount = SubtotalAmount.Add(TaxAmount);
    }

    private static string GenerateInvoiceNumber()
    {
        return $"INV-{DateTimeOffset.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
    }

    public bool IsOverdue => Status == InvoiceStatus.Sent && DateTimeOffset.UtcNow > DueDate;
    public int DaysOverdue => IsOverdue ? (DateTimeOffset.UtcNow - DueDate).Days : 0;
}

public class InvoiceLineItem
{
    public string Description { get; private set; } = string.Empty;
    public Money UnitPrice { get; private set; } = Money.Zero("USD");
    public int Quantity { get; private set; }
    public Money TotalPrice { get; private set; } = Money.Zero("USD");

    private InvoiceLineItem() { } // EF Core

    public InvoiceLineItem(string description, Money unitPrice, int quantity)
    {
        Update(description, unitPrice, quantity);
    }

    public void Update(string description, Money unitPrice, int quantity)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be null or empty", nameof(description));

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        Description = description.Trim();
        UnitPrice = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));
        Quantity = quantity;
        TotalPrice = UnitPrice.Multiply(quantity);
    }
}

public enum InvoiceStatus
{
    Draft = 0,
    Sent = 1,
    Paid = 2,
    Cancelled = 3
}
