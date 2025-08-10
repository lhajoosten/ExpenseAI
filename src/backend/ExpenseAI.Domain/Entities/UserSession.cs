using ExpenseAI.Domain.Common;

namespace ExpenseAI.Domain.Entities;

public class UserSession : BaseEntity
{
    public Guid UserId { get; private set; }
    public string DeviceInfo { get; private set; } = string.Empty;
    public string IpAddress { get; private set; } = string.Empty;
    public string RefreshToken { get; private set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset? LastActiveAt { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Navigation properties
    public User User { get; private set; } = null!;

    private UserSession() { } // EF Core

    public UserSession(
        Guid userId,
        string deviceInfo,
        string ipAddress,
        string refreshToken,
        DateTimeOffset expiresAt)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        if (string.IsNullOrWhiteSpace(deviceInfo))
            throw new ArgumentException("Device info cannot be null or empty", nameof(deviceInfo));

        if (string.IsNullOrWhiteSpace(ipAddress))
            throw new ArgumentException("IP address cannot be null or empty", nameof(ipAddress));

        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new ArgumentException("Refresh token cannot be null or empty", nameof(refreshToken));

        UserId = userId;
        DeviceInfo = deviceInfo.Trim();
        IpAddress = ipAddress.Trim();
        RefreshToken = refreshToken;
        ExpiresAt = expiresAt;
        LastActiveAt = DateTimeOffset.UtcNow;
    }

    public void UpdateLastActive()
    {
        LastActiveAt = DateTimeOffset.UtcNow;
        MarkAsUpdated();
    }

    public void Revoke()
    {
        IsActive = false;
        MarkAsUpdated();
    }

    public bool IsExpired => DateTimeOffset.UtcNow > ExpiresAt;
    public bool IsValidForRefresh => IsActive && !IsExpired;
}
