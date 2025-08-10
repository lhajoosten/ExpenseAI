namespace ExpenseAI.Application.Queries.Auth.DTOs;

public class UserSessionDto
{
    public Guid Id { get; set; }
    public string DeviceInfo { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? LastActiveAt { get; set; }
    public bool IsActive { get; set; }
    public bool IsCurrent { get; set; }
}
