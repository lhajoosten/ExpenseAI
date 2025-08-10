namespace ExpenseAI.Application.Interfaces;

/// <summary>
/// Unit of Work interface for managing repository transactions
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Expense repository
    /// </summary>
    IExpenseRepository Expenses { get; }

    /// <summary>
    /// Category repository
    /// </summary>
    ICategoryRepository Categories { get; }

    /// <summary>
    /// Invoice repository
    /// </summary>
    IInvoiceRepository Invoices { get; }

    /// <summary>
    /// Budget repository
    /// </summary>
    IBudgetRepository Budgets { get; }

    /// <summary>
    /// User repository
    /// </summary>
    IUserRepository Users { get; }

    /// <summary>
    /// Save all changes asynchronously
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begin a database transaction
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commit the current transaction
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rollback the current transaction
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
