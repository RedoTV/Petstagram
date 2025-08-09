namespace Petsgram.Domain.Entities;

public class Token
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }

    public Token(string accessToken, string refreshToken, DateTime createdAt, DateTime expiresAt)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
    }

    public bool IsExpired => DateTime.UtcNow > ExpiresAt;
}