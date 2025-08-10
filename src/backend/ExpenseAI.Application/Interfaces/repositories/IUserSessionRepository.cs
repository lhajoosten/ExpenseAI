using ExpenseAI.Domain.Entities;

namespace ExpenseAI.Application.Interfaces;

public interface IUserSessionRepository : IBaseRepository<UserSession>
{
    Task<IReadOnlyList<UserSession>> GetActiveSessionsByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserSession?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task InvalidateSessionAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task InvalidateAllUserSessionsAsync(Guid userId, CancellationToken cancellationToken = default);
}
