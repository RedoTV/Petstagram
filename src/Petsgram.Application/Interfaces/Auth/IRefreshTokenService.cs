using Petsgram.Domain.Entities;

namespace Petsgram.Application.Interfaces.Auth;

public interface IRefreshTokenService
{
    Task<Token> RefreshTokenAsync(string accessToken, string refreshToken, CancellationToken cancellationToken = default);
    Task<bool> ValidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<User?> GetUserFromRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task StoreRefreshToken(int userId, string refreshToken, CancellationToken cancellationToken = default);
    Task RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task RevokeAllUserTokensAsync(int userId, CancellationToken cancellationToken = default);
    Task CleanupExpiredTokensAsync(CancellationToken cancellationToken = default);
}