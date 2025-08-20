using Petsgram.Application.DTOs.Users;
using Petsgram.Domain.Entities;

namespace Petsgram.Application.Interfaces.Auth;

public interface IRefreshTokenService
{
    Task<Token> RefreshTokenAsync(string accessToken, string refreshToken, CancellationToken cancellationToken = default);
    Task<bool> ValidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<User?> GetUserFromRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<RefreshTokenResponse> StoreRefreshToken(int userId, string refreshToken, CancellationToken cancellationToken = default);
    Task<RefreshTokenResponse> RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<List<RefreshTokenResponse>> RevokeAllUserTokensAsync(int userId, CancellationToken cancellationToken = default);
    Task CleanupExpiredTokensAsync(CancellationToken cancellationToken = default);
}