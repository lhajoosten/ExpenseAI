using Microsoft.EntityFrameworkCore.Storage;
using ExpenseAI.Application.Interfaces;
using ExpenseAI.Infrastructure.Data;

namespace ExpenseAI.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Expenses = new ExpenseRepository(_context);
        Categories = new CategoryRepository(_context);
        Invoices = new InvoiceRepository(_context);
        Budgets = new BudgetRepository(_context);
        Users = new UserRepository(_context);
    }

    public IExpenseRepository Expenses { get; }
    public ICategoryRepository Categories { get; }
    public IInvoiceRepository Invoices { get; }
    public IBudgetRepository Budgets { get; }
    public IUserRepository Users { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
