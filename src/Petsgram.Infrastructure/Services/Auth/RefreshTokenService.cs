using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Petsgram.Application.Interfaces.Auth;
using Petsgram.Application.Interfaces.Users;
using Petsgram.Application.Settings;
using Petsgram.Domain.Entities;

namespace Petsgram.Infrastructure.Services.Auth;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly AuthSettings _authSettings;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public RefreshTokenService(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IOptions<AuthSettings> authSettings)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _authSettings = authSettings.Value;
    }

    public async Task<Token> RefreshTokenAsync(string accessToken, string refreshToken, CancellationToken cancellationToken = default)
    {
        var principal = GetPrincipalFromExpiredToken(accessToken);
        if (principal?.Identity?.Name == null)
            throw new SecurityTokenException("Invalid access token");

        var user = await _userRepository.GetByUserNameAsync(principal.Identity.Name, cancellationToken);
        if (user == null)
            throw new SecurityTokenException("User not found");

        var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);

        if (storedToken == null)
        {

            throw new SecurityTokenException("Invalid refresh token: token not found");
        }



        if (storedToken.UserId != user.Id)
        {

            throw new SecurityTokenException("Invalid refresh token: wrong user");
        }

        if (storedToken.IsExpired)
        {

            throw new SecurityTokenException("Invalid refresh token: expired");
        }

        if (storedToken.IsRevoked)
        {

            throw new SecurityTokenException("Invalid refresh token: revoked");
        }



        var newJwtToken = GenerateJwtToken(user);
        var newRefreshToken = GenerateRefreshToken();

        storedToken.IsRevoked = true;
        await _refreshTokenRepository.UpdateAsync(storedToken, cancellationToken);

        var newRefreshTokenEntity = new RefreshToken
        {
            Token = newRefreshToken,
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(int.Parse(_authSettings.RefreshTokenExpireDays)),
            IsRevoked = false
        };
        await _refreshTokenRepository.AddAsync(newRefreshTokenEntity, cancellationToken);

        return new Token(newJwtToken, newRefreshToken, DateTime.UtcNow, DateTime.UtcNow.AddMinutes(GetJwtExpirationMinutes()));
    }


    public async Task<bool> ValidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);
        return storedToken?.IsActive == true;
    }

    public async Task<User?> GetUserFromRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);
        if (storedToken?.IsActive != true)
            return null;
        return await _userRepository.FindAsync(storedToken.UserId, cancellationToken);
    }

    public async Task StoreRefreshToken(int userId, string refreshToken, CancellationToken cancellationToken = default)
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
        await _refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);
    }

    public async Task RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        await _refreshTokenRepository.RevokeTokenAsync(refreshToken, cancellationToken);
    }

    public async Task RevokeAllUserTokensAsync(int userId, CancellationToken cancellationToken = default)
    {
        await _refreshTokenRepository.RevokeAllUserTokensAsync(userId, cancellationToken);
    }

    public async Task CleanupExpiredTokensAsync(CancellationToken cancellationToken = default)
    {
        await _refreshTokenRepository.CleanupExpiredTokensAsync(cancellationToken);
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Role, user.Role.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_authSettings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _authSettings.Issuer,
            audience: _authSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(GetJwtExpirationMinutes()),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = _authSettings.Audience,
            ValidateIssuer = true,
            ValidIssuer = _authSettings.Issuer,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_authSettings.SecretKey)),
            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);


            return principal;
        }
        catch (Exception ex)
        {

            throw new SecurityTokenException("Invalid token");
        }
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private int GetJwtExpirationMinutes()
    {
        var days = int.Parse(_authSettings.AccessTokenExpireDays);
        return days * 24 * 60;
    }
}
