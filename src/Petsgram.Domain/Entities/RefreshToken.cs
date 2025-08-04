namespace Petsgram.Domain.Entities;

public class RefreshToken
{
    public int Id { get; set; }
    public required string Token { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public bool IsExpired => DateTime.UtcNow > ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;
}