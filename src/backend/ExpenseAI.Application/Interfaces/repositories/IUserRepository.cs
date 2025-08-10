using ExpenseAI.Domain.Entities;

namespace ExpenseAI.Application.Interfaces;

/// <summary>
/// Repository interface for user operations
/// </summary>
public interface IUserRepository : IBaseRepository<ExpenseAIIdentityUser>
{
    /// <summary>
    /// Get user by email
    /// </summary>
    Task<ExpenseAIIdentityUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get user by username
    /// </summary>
    Task<ExpenseAIIdentityUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if email exists
    /// </summary>
    Task<bool> EmailExistsAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if username exists
    /// </summary>
    Task<bool> UsernameExistsAsync(string username, Guid? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get active users
    /// </summary>
    Task<IReadOnlyList<ExpenseAIIdentityUser>> GetActiveUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get users by creation date range
    /// </summary>
    Task<IReadOnlyList<ExpenseAIIdentityUser>> GetByCreationDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Search users by name or email
    /// </summary>
    Task<IReadOnlyList<ExpenseAIIdentityUser>> SearchAsync(
        string searchTerm,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get users with recent activity
    /// </summary>
    Task<IReadOnlyList<ExpenseAIIdentityUser>> GetRecentlyActiveAsync(
        int days = 30,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Update last login time
    /// </summary>
    Task UpdateLastLoginAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update refresh token
    /// </summary>
    Task UpdateRefreshTokenAsync(
        Guid userId,
        string refreshToken,
        DateTime expiryTime,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Clear refresh token
    /// </summary>
    Task ClearRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get user by refresh token
    /// </summary>
    Task<ExpenseAIIdentityUser?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}
