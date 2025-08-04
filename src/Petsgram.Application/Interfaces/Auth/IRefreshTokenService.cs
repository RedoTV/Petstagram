using Petsgram.Domain.Entities;

namespace Petsgram.Application.Interfaces.Auth;

public interface IRefreshTokenService
{
    Task<Token> RefreshTokenAsync(string refreshToken);
    Task<bool> ValidateRefreshTokenAsync(string refreshToken);
    Task<User?> GetUserFromRefreshTokenAsync(string refreshToken);
    Task StoreRefreshToken(int userId, string refreshToken);
    Task RevokeTokenAsync(string refreshToken);
    Task RevokeAllUserTokensAsync(int userId);
    Task CleanupExpiredTokensAsync();
}