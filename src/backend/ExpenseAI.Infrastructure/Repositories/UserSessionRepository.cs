using Microsoft.EntityFrameworkCore;
using ExpenseAI.Application.Interfaces;
using ExpenseAI.Domain.Entities;
using ExpenseAI.Infrastructure.Data;

namespace ExpenseAI.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for user session operations
/// </summary>
public class UserSessionRepository : BaseRepository<UserSession>, IUserSessionRepository
{
    public UserSessionRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Get active sessions by user ID
    /// </summary>
    public async Task<IReadOnlyList<UserSession>> GetActiveSessionsByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.UserId == userId && s.IsActive && s.ExpiresAt > DateTimeOffset.UtcNow)
            .OrderByDescending(s => s.LastActiveAt ?? s.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get session by refresh token
    /// </summary>
    public async Task<UserSession?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => s.RefreshToken == token && s.IsActive && s.ExpiresAt > DateTimeOffset.UtcNow, cancellationToken);
    }

    /// <summary>
    /// Invalidate a specific session
    /// </summary>
    public async Task InvalidateSessionAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var session = await _dbSet.FindAsync(new object[] { sessionId }, cancellationToken);
        if (session != null)
        {
            session.Revoke();
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Invalidate all sessions for a user
    /// </summary>
    public async Task InvalidateAllUserSessionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var sessions = await _dbSet
            .Where(s => s.UserId == userId && s.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var session in sessions)
        {
            session.Revoke();
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
