using Microsoft.EntityFrameworkCore;
using ExpenseAI.Application.Interfaces;
using ExpenseAI.Domain.Entities;
using ExpenseAI.Domain.ValueObjects;
using ExpenseAI.Infrastructure.Data;

namespace ExpenseAI.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for invoice operations
/// </summary>
public class InvoiceRepository : BaseRepository<Invoice>, IInvoiceRepository
{
    public InvoiceRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Get invoices by user ID
    /// </summary>
    public async Task<IReadOnlyList<Invoice>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(i => i.UserId == userId)
            .OrderByDescending(i => i.IssueDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get invoices by user ID with pagination
    /// </summary>
    public async Task<(IReadOnlyList<Invoice> Items, int TotalCount)> GetByUserIdAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(i => i.UserId == userId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(i => i.IssueDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    /// <summary>
    /// Get invoice by invoice number
    /// </summary>
    public async Task<Invoice?> GetByInvoiceNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(i => i.InvoiceNumber == invoiceNumber, cancellationToken);
    }

    /// <summary>
    /// Get invoices by status
    /// </summary>
    public async Task<IReadOnlyList<Invoice>> GetByStatusAsync(
        Guid userId,
        InvoiceStatus status,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(i => i.UserId == userId && i.Status == status)
            .OrderByDescending(i => i.IssueDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get invoices by date range
    /// </summary>
    public async Task<IReadOnlyList<Invoice>> GetByDateRangeAsync(
        Guid userId,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(i => i.UserId == userId &&
                       i.IssueDate >= startDate &&
                       i.IssueDate <= endDate)
            .OrderByDescending(i => i.IssueDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get overdue invoices
    /// </summary>
    public async Task<IReadOnlyList<Invoice>> GetOverdueAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        return await _dbSet
            .Where(i => i.UserId == userId &&
                       i.Status == InvoiceStatus.Sent &&
                       i.DueDate < now)
            .OrderByDescending(i => i.DueDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get draft invoices
    /// </summary>
    public async Task<IReadOnlyList<Invoice>> GetDraftsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(i => i.UserId == userId && i.Status == InvoiceStatus.Draft)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get paid invoices
    /// </summary>
    public async Task<IReadOnlyList<Invoice>> GetPaidAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(i => i.UserId == userId && i.Status == InvoiceStatus.Paid)
            .OrderByDescending(i => i.PaidDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get total revenue by user
    /// </summary>
    public async Task<Money> GetTotalRevenueAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var invoices = await _dbSet
            .Where(i => i.UserId == userId && i.Status == InvoiceStatus.Paid)
            .Select(i => i.TotalAmount)
            .ToListAsync(cancellationToken);

        if (!invoices.Any())
            return Money.Zero("USD");

        // Group by currency and sum
        var groupedByCurrency = invoices
            .GroupBy(m => m.Currency)
            .Select(g => new Money(g.Sum(m => m.Amount), g.Key))
            .ToList();

        // For simplicity, return the first currency group or convert to USD
        return groupedByCurrency.FirstOrDefault() ?? Money.Zero("USD");
    }

    /// <summary>
    /// Get total revenue by user and date range
    /// </summary>
    public async Task<Money> GetTotalRevenueByDateRangeAsync(
        Guid userId,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        CancellationToken cancellationToken = default)
    {
        var invoices = await _dbSet
            .Where(i => i.UserId == userId &&
                       i.Status == InvoiceStatus.Paid &&
                       i.PaidDate >= startDate &&
                       i.PaidDate <= endDate)
            .Select(i => i.TotalAmount)
            .ToListAsync(cancellationToken);

        if (!invoices.Any())
            return Money.Zero("USD");

        // Group by currency and sum
        var groupedByCurrency = invoices
            .GroupBy(m => m.Currency)
            .Select(g => new Money(g.Sum(m => m.Amount), g.Key))
            .ToList();

        // For simplicity, return the first currency group or convert to USD
        return groupedByCurrency.FirstOrDefault() ?? Money.Zero("USD");
    }

    /// <summary>
    /// Get invoices by client email
    /// </summary>
    public async Task<IReadOnlyList<Invoice>> GetByClientEmailAsync(
        Guid userId,
        string clientEmail,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(i => i.UserId == userId && i.ClientEmail.ToLower() == clientEmail.ToLower())
            .OrderByDescending(i => i.IssueDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Search invoices by client name or invoice number
    /// </summary>
    public async Task<IReadOnlyList<Invoice>> SearchAsync(
        Guid userId,
        string searchTerm,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetByUserIdAsync(userId, cancellationToken);

        var lowerSearchTerm = searchTerm.ToLower();

        return await _dbSet
            .Where(i => i.UserId == userId &&
                       (i.ClientName.ToLower().Contains(lowerSearchTerm) ||
                        i.InvoiceNumber.ToLower().Contains(lowerSearchTerm) ||
                        (i.Notes != null && i.Notes.ToLower().Contains(lowerSearchTerm))))
            .OrderByDescending(i => i.IssueDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get next invoice number
    /// </summary>
    public async Task<string> GetNextInvoiceNumberAsync(CancellationToken cancellationToken = default)
    {
        var year = DateTime.UtcNow.Year;
        var yearPrefix = $"INV-{year}-";

        var lastInvoice = await _dbSet
            .Where(i => i.InvoiceNumber.StartsWith(yearPrefix))
            .OrderByDescending(i => i.InvoiceNumber)
            .FirstOrDefaultAsync(cancellationToken);

        if (lastInvoice == null)
        {
            return $"{yearPrefix}0001";
        }

        // Extract the sequential number from the last invoice
        var lastNumber = lastInvoice.InvoiceNumber.Substring(yearPrefix.Length);
        if (int.TryParse(lastNumber, out var number))
        {
            return $"{yearPrefix}{(number + 1):D4}";
        }

        return $"{yearPrefix}0001";
    }

    /// <summary>
    /// Check if invoice number exists
    /// </summary>
    public async Task<bool> InvoiceNumberExistsAsync(string invoiceNumber, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(i => i.InvoiceNumber == invoiceNumber);

        if (excludeId.HasValue)
        {
            query = query.Where(i => i.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Get total amount by user
    /// </summary>
    public async Task<Money> GetTotalByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var invoices = await _dbSet
            .Where(i => i.UserId == userId && i.Status != InvoiceStatus.Cancelled)
            .Select(i => i.TotalAmount)
            .ToListAsync(cancellationToken);

        if (!invoices.Any())
            return Money.Zero("USD");

        // Group by currency and sum
        var groupedByCurrency = invoices
            .GroupBy(m => m.Currency)
            .Select(g => new Money(g.Sum(m => m.Amount), g.Key))
            .ToList();

        // For simplicity, return the first currency group or convert to USD
        return groupedByCurrency.FirstOrDefault() ?? Money.Zero("USD");
    }

    /// <summary>
    /// Get total amount by date range
    /// </summary>
    public async Task<Money> GetTotalByDateRangeAsync(
        Guid userId,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        CancellationToken cancellationToken = default)
    {
        var invoices = await _dbSet
            .Where(i => i.UserId == userId &&
                       i.IssueDate >= startDate &&
                       i.IssueDate <= endDate &&
                       i.Status != InvoiceStatus.Cancelled)
            .Select(i => i.TotalAmount)
            .ToListAsync(cancellationToken);

        if (!invoices.Any())
            return Money.Zero("USD");

        // Group by currency and sum
        var groupedByCurrency = invoices
            .GroupBy(m => m.Currency)
            .Select(g => new Money(g.Sum(m => m.Amount), g.Key))
            .ToList();

        // For simplicity, return the first currency group or convert to USD
        return groupedByCurrency.FirstOrDefault() ?? Money.Zero("USD");
    }

    /// <summary>
    /// Search invoices by client or description
    /// </summary>
    public async Task<IReadOnlyList<Invoice>> SearchAsync(
        Guid userId,
        string? searchTerm = null,
        InvoiceStatus? status = null,
        decimal? minAmount = null,
        decimal? maxAmount = null,
        DateTimeOffset? startDate = null,
        DateTimeOffset? endDate = null,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(i => i.UserId == userId);

        // Apply search term filter
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLower();
            query = query.Where(i => i.ClientName.ToLower().Contains(lowerSearchTerm) ||
                                   (i.Notes != null && i.Notes.ToLower().Contains(lowerSearchTerm)));
        }

        // Apply status filter
        if (status.HasValue)
        {
            query = query.Where(i => i.Status == status.Value);
        }

        // Apply amount filters
        if (minAmount.HasValue)
        {
            query = query.Where(i => i.TotalAmount.Amount >= minAmount.Value);
        }

        if (maxAmount.HasValue)
        {
            query = query.Where(i => i.TotalAmount.Amount <= maxAmount.Value);
        }

        // Apply date range filters
        if (startDate.HasValue)
        {
            query = query.Where(i => i.IssueDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(i => i.IssueDate <= endDate.Value);
        }

        return await query
            .OrderByDescending(i => i.IssueDate)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get invoices pending payment
    /// </summary>
    public async Task<IReadOnlyList<Invoice>> GetPendingPaymentAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(i => i.UserId == userId && i.Status == InvoiceStatus.Sent)
            .OrderByDescending(i => i.IssueDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get recent invoices
    /// </summary>
    public async Task<IReadOnlyList<Invoice>> GetRecentAsync(
        Guid userId,
        int count = 10,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(i => i.UserId == userId)
            .OrderByDescending(i => i.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
}
