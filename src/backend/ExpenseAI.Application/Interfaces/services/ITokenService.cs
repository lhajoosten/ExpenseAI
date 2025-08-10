using ExpenseAI.Domain.Entities;

namespace ExpenseAI.Application.Interfaces;

public interface ITokenService
{
    Task<TokenResult> GenerateAccessTokenAsync(ExpenseAIIdentityUser user);
    Task<TokenResult> GenerateRefreshTokenAsync(ExpenseAIIdentityUser user);
    Task<RefreshResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}

public record TokenResult(string Token, DateTime ExpiresAt);

public record RefreshResult(string AccessToken, string RefreshToken, DateTime ExpiresAt, bool IsValid);
