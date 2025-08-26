namespace Petsgram.Application.DTOs.Users;

public class RevokeTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}