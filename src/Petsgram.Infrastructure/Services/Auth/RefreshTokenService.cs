using Petsgram.Application.Interfaces.Auth;
using Petsgram.Application.Generators;
using Petsgram.Domain.Entities;
using Petsgram.Application.Interfaces.Users;
using Petsgram.Application.Settings;
using Microsoft.Extensions.Options;

namespace Petsgram.Infrastructure.Services.Auth;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly AuthSettings _authSettings;

    public RefreshTokenService(
        ITokenGenerator tokenGenerator,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IOptions<AuthSettings> authSettings)
    {
        _tokenGenerator = tokenGenerator;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _authSettings = authSettings.Value;
    }

    public async Task<Token> RefreshTokenAsync(string refreshToken)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
        if (storedToken == null)
            throw new InvalidOperationException("Invalid refresh token");

        if (!storedToken.IsActive)
            throw new InvalidOperationException("Refresh token is not active");

        var user = await _userRepository.FindAsync(storedToken.UserId);
        if (user == null)
            throw new InvalidOperationException("User not found");

        var newToken = _tokenGenerator.GenerateToken(user);

        await _refreshTokenRepository.RevokeTokenAsync(refreshToken);

        await StoreRefreshToken(user.Id, newToken.RefreshToken);

        return newToken;
    }

    public async Task<bool> ValidateRefreshTokenAsync(string refreshToken)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
        return storedToken?.IsActive == true;
    }

    public async Task<User?> GetUserFromRefreshTokenAsync(string refreshToken)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
        if (storedToken?.IsActive != true)
            return null;

        return await _userRepository.FindAsync(storedToken.UserId);
    }

    public async Task StoreRefreshToken(int userId, string refreshToken)
    {
        var expiresAt = DateTime.UtcNow.AddDays(int.Parse(_authSettings.RefreshTokenExpireDays));

        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt,
            IsRevoked = false
        };

        await _refreshTokenRepository.AddAsync(refreshTokenEntity);
    }

    public async Task RevokeTokenAsync(string refreshToken)
    {
        await _refreshTokenRepository.RevokeTokenAsync(refreshToken);
    }

    public async Task RevokeAllUserTokensAsync(int userId)
    {
        await _refreshTokenRepository.RevokeAllUserTokensAsync(userId);
    }

    public async Task CleanupExpiredTokensAsync()
    {
        await _refreshTokenRepository.CleanupExpiredTokensAsync();
    }
}