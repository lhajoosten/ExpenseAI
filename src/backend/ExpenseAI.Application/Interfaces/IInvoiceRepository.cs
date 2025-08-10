using ExpenseAI.Domain.Entities;
using ExpenseAI.Domain.ValueObjects;

namespace ExpenseAI.Application.Interfaces;

/// <summary>
/// Repository interface for invoice operations
/// </summary>
public interface IInvoiceRepository : IBaseRepository<Invoice>
{
    /// <summary>
    /// Get invoices by user ID
    /// </summary>
    Task<IReadOnlyList<Invoice>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get invoices by user ID with pagination
    /// </summary>
    Task<(IReadOnlyList<Invoice> Items, int TotalCount)> GetByUserIdAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get invoice by invoice number
    /// </summary>
    Task<Invoice?> GetByInvoiceNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get invoices by status
    /// </summary>
    Task<IReadOnlyList<Invoice>> GetByStatusAsync(
        Guid userId,
        InvoiceStatus status,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get invoices by date range
    /// </summary>
    Task<IReadOnlyList<Invoice>> GetByDateRangeAsync(
        Guid userId,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get overdue invoices
    /// </summary>
    Task<IReadOnlyList<Invoice>> GetOverdueAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get draft invoices
    /// </summary>
    Task<IReadOnlyList<Invoice>> GetDraftsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get paid invoices
    /// </summary>
    Task<IReadOnlyList<Invoice>> GetPaidAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get total revenue by user
    /// </summary>
    Task<Money> GetTotalRevenueAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get total revenue by user and date range
    /// </summary>
    Task<Money> GetTotalRevenueByDateRangeAsync(
        Guid userId,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get invoices by client email
    /// </summary>
    Task<IReadOnlyList<Invoice>> GetByClientEmailAsync(
        Guid userId,
        string clientEmail,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Search invoices by client name or invoice number
    /// </summary>
    Task<IReadOnlyList<Invoice>> SearchAsync(
        Guid userId,
        string searchTerm,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get next invoice number
    /// </summary>
    Task<string> GetNextInvoiceNumberAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if invoice number exists
    /// </summary>
    Task<bool> InvoiceNumberExistsAsync(string invoiceNumber, Guid? excludeId = null, CancellationToken cancellationToken = default);
}
