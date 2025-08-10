using Microsoft.EntityFrameworkCore;
using ExpenseAI.Application.Interfaces;
using ExpenseAI.Domain.Entities;
using ExpenseAI.Infrastructure.Data;

namespace ExpenseAI.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for user operations
/// </summary>
public class UserRepository : BaseRepository<ExpenseAIIdentityUser>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Get user by email
    /// </summary>
    public async Task<ExpenseAIIdentityUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email != null && u.Email.ToLower() == email.ToLower(), cancellationToken);
    }

    /// <summary>
    /// Get user by username
    /// </summary>
    public async Task<ExpenseAIIdentityUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username))
            return null;

        return await _dbSet
            .FirstOrDefaultAsync(u => u.UserName != null && u.UserName.ToLower() == username.ToLower(), cancellationToken);
    }

    /// <summary>
    /// Check if email exists
    /// </summary>
    public async Task<bool> EmailExistsAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var query = _dbSet.Where(u => u.Email != null && u.Email.ToLower() == email.ToLower());

        if (excludeId.HasValue)
        {
            query = query.Where(u => u.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Check if username exists
    /// </summary>
    public async Task<bool> UsernameExistsAsync(string username, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username))
            return false;

        var query = _dbSet.Where(u => u.UserName != null && u.UserName.ToLower() == username.ToLower());

        if (excludeId.HasValue)
        {
            query = query.Where(u => u.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Get active users
    /// </summary>
    public async Task<IReadOnlyList<ExpenseAIIdentityUser>> GetActiveUsersAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(u => u.IsActive)
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get users by creation date range
    /// </summary>
    public async Task<IReadOnlyList<ExpenseAIIdentityUser>> GetByCreationDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(u => u.CreatedAt >= startDate && u.CreatedAt <= endDate)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Search users by name or email
    /// </summary>
    public async Task<IReadOnlyList<ExpenseAIIdentityUser>> SearchAsync(
        string searchTerm,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetActiveUsersAsync(cancellationToken);

        var lowerSearchTerm = searchTerm.ToLower();

        return await _dbSet
            .Where(u => u.IsActive &&
                       (u.FirstName.ToLower().Contains(lowerSearchTerm) ||
                        u.LastName.ToLower().Contains(lowerSearchTerm) ||
                        (u.Email != null && u.Email.ToLower().Contains(lowerSearchTerm))))
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get users with recent activity
    /// </summary>
    public async Task<IReadOnlyList<ExpenseAIIdentityUser>> GetRecentlyActiveAsync(
        int days = 30,
        CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-days);

        return await _dbSet
            .Where(u => u.IsActive &&
                       u.LastLoginAt.HasValue &&
                       u.LastLoginAt >= cutoffDate)
            .OrderByDescending(u => u.LastLoginAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Update last login time
    /// </summary>
    public async Task UpdateLastLoginAsync(Guid userId, CancellationToken cancellationToken = default)
    {

        var user = await _dbSet
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user != null)
        {
            user.LastLoginAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Update refresh token
    /// </summary>
    public async Task UpdateRefreshTokenAsync(
        Guid userId,
        string refreshToken,
        DateTime expiryTime,
        CancellationToken cancellationToken = default)
    {

        var user = await _dbSet
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user != null)
        {
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = expiryTime;
            user.UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Clear refresh token
    /// </summary>
    public async Task ClearRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default)
    {

        var user = await _dbSet
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user != null)
        {
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            user.UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Get user by refresh token
    /// </summary>
    public async Task<ExpenseAIIdentityUser?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return null;

        return await _dbSet
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken &&
                               u.RefreshTokenExpiryTime.HasValue &&
                               u.RefreshTokenExpiryTime > DateTime.UtcNow,
                               cancellationToken);
    }
}
