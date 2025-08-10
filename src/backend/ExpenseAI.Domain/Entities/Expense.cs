using ExpenseAI.Domain.Common;
using ExpenseAI.Domain.ValueObjects;
using ExpenseAI.Domain.Events;
using CategoryVO = ExpenseAI.Domain.ValueObjects.Category;

namespace ExpenseAI.Domain.Entities;

public class Expense : BaseEntity
{
    public string Description { get; private set; } = string.Empty;
    public Money Amount { get; private set; } = Money.Zero("USD");
    public CategoryVO Category { get; private set; } = CategoryVO.Uncategorized;
    public DateTimeOffset ExpenseDate { get; private set; }
    public string? Notes { get; private set; }
    public string? ReceiptUrl { get; private set; }
    public string? MerchantName { get; private set; }
    public string? PaymentMethod { get; private set; }
    public bool IsReimbursable { get; private set; }
    public bool IsReimbursed { get; private set; }
    public ExpenseStatus Status { get; private set; } = ExpenseStatus.Draft;
    public string? RejectionReason { get; private set; }

    // AI-related properties
    public bool IsAiCategorized { get; private set; }
    public double? AiConfidenceScore { get; private set; }
    public string? AiExtractedData { get; private set; }

    // Foreign keys
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    // Tags for flexible categorization
    private readonly List<string> _tags = new();
    public IReadOnlyList<string> Tags => _tags.AsReadOnly();

    private Expense() { } // EF Core

    public Expense(
        Guid userId,
        string description,
        Money amount,
        CategoryVO category,
        DateTimeOffset expenseDate,
        string? notes = null,
        string? merchantName = null,
        string? paymentMethod = null,
        bool isReimbursable = false)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be null or empty", nameof(description));

        if (description.Length > 500)
            throw new ArgumentException("Description cannot exceed 500 characters", nameof(description));

        UserId = userId;
        Description = description.Trim();
        Amount = amount ?? throw new ArgumentNullException(nameof(amount));
        Category = category ?? throw new ArgumentNullException(nameof(category));
        ExpenseDate = expenseDate;
        Notes = notes?.Trim();
        MerchantName = merchantName?.Trim();
        PaymentMethod = paymentMethod?.Trim();
        IsReimbursable = isReimbursable;

        AddDomainEvent(new ExpenseCreatedEvent(Id, UserId, Amount.Amount, Amount.Currency));
    }

    public void UpdateDetails(
        string description,
        Money amount,
        CategoryVO category,
        DateTimeOffset expenseDate,
        string? notes = null,
        string? merchantName = null,
        string? paymentMethod = null,
        bool? isReimbursable = null)
    {
        if (Status == ExpenseStatus.Approved)
            throw new InvalidOperationException("Cannot update approved expenses");

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be null or empty", nameof(description));

        var oldAmount = Amount.Amount;

        Description = description.Trim();
        Amount = amount ?? throw new ArgumentNullException(nameof(amount));
        Category = category ?? throw new ArgumentNullException(nameof(category));
        ExpenseDate = expenseDate;
        Notes = notes?.Trim();
        MerchantName = merchantName?.Trim();
        PaymentMethod = paymentMethod?.Trim();

        if (isReimbursable.HasValue)
            IsReimbursable = isReimbursable.Value;

        MarkAsUpdated();

        if (oldAmount != Amount.Amount)
        {
            AddDomainEvent(new ExpenseUpdatedEvent(Id, UserId, oldAmount, Amount.Amount, Amount.Currency));
        }
    }

    public void AttachReceipt(string receiptUrl)
    {
        if (string.IsNullOrWhiteSpace(receiptUrl))
            throw new ArgumentException("Receipt URL cannot be null or empty", nameof(receiptUrl));

        ReceiptUrl = receiptUrl;
        MarkAsUpdated();
    }

    public void SetAiCategorization(CategoryVO category, double confidenceScore, string? extractedData = null)
    {
        if (confidenceScore < 0 || confidenceScore > 1)
            throw new ArgumentException("Confidence score must be between 0 and 1", nameof(confidenceScore));

        Category = category ?? throw new ArgumentNullException(nameof(category));
        IsAiCategorized = true;
        AiConfidenceScore = confidenceScore;
        AiExtractedData = extractedData;
        MarkAsUpdated();
    }

    public void AddTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
            throw new ArgumentException("Tag cannot be null or empty", nameof(tag));

        var normalizedTag = tag.Trim().ToLowerInvariant();
        if (!_tags.Contains(normalizedTag))
        {
            _tags.Add(normalizedTag);
            MarkAsUpdated();
        }
    }

    public void RemoveTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
            return;

        var normalizedTag = tag.Trim().ToLowerInvariant();
        if (_tags.Remove(normalizedTag))
        {
            MarkAsUpdated();
        }
    }

    public void Submit()
    {
        if (Status != ExpenseStatus.Draft)
            throw new InvalidOperationException($"Cannot submit expense in {Status} status");

        Status = ExpenseStatus.Submitted;
        MarkAsUpdated();
    }

    public void Approve()
    {
        if (Status != ExpenseStatus.Submitted)
            throw new InvalidOperationException($"Cannot approve expense in {Status} status");

        Status = ExpenseStatus.Approved;
        RejectionReason = null;
        MarkAsUpdated();
    }

    public void Reject(string reason)
    {
        if (Status != ExpenseStatus.Submitted)
            throw new InvalidOperationException($"Cannot reject expense in {Status} status");

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Rejection reason cannot be null or empty", nameof(reason));

        Status = ExpenseStatus.Rejected;
        RejectionReason = reason.Trim();
        MarkAsUpdated();
    }

    public void MarkAsReimbursed()
    {
        if (!IsReimbursable)
            throw new InvalidOperationException("Cannot mark non-reimbursable expense as reimbursed");

        if (Status != ExpenseStatus.Approved)
            throw new InvalidOperationException("Cannot reimburse non-approved expense");

        IsReimbursed = true;
        MarkAsUpdated();
    }
}

public enum ExpenseStatus
{
    Draft = 0,
    Submitted = 1,
    Approved = 2,
    Rejected = 3
}
