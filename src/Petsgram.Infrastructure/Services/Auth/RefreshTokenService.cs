using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Petsgram.Application.DTOs.Users;
using Petsgram.Application.Interfaces.Auth;
using Petsgram.Application.Interfaces.UnitOfWork;
using Petsgram.Application.Interfaces.Users;
using Petsgram.Application.Settings;
using Petsgram.Domain.Entities;
using Petsgram.Domain.Exceptions.Auth;

namespace Petsgram.Infrastructure.Services.Auth;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<RefreshTokenService> _logger;
    private readonly AuthSettings _authSettings;

    public RefreshTokenService(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<RefreshTokenService> logger,
        IOptions<AuthSettings> authSettings)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _authSettings = authSettings.Value;
    }

    public async Task<Token> RefreshTokenAsync(string accessToken, string refreshToken, CancellationToken cancellationToken = default)
    {
        var principal = GetPrincipalFromExpiredToken(accessToken);
        if (principal?.Identity?.Name == null)
        {
            _logger.LogWarning("Invalid access token provided for refresh");
            throw new TokenValidationException("Invalid access token");
        }

        var user = await _userRepository.GetByUserNameAsync(principal.Identity.Name, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("User not found for token refresh: {UserName}", principal.Identity.Name);
            throw new TokenValidationException("User not found");
        }

        var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);
        if (storedToken == null)
        {
            _logger.LogWarning("Refresh token not found for user {UserId}", user.Id);
            throw new TokenValidationException("Invalid refresh token");
        }

        if (storedToken.UserId != user.Id)
        {
            _logger.LogWarning("Refresh token belongs to different user. Expected: {ExpectedUserId}, Actual: {ActualUserId}", user.Id, storedToken.UserId);
            throw new TokenValidationException("Invalid refresh token");
        }

        if (storedToken.IsExpired)
        {
            _logger.LogWarning("Expired refresh token used for user {UserId}", user.Id);
            throw new TokenValidationException("Refresh token expired");
        }

        if (storedToken.IsRevoked)
        {
            _logger.LogWarning("Revoked refresh token used for user {UserId}", user.Id);
            throw new TokenValidationException("Refresh token revoked");
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
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Token refreshed successfully for user {UserId}", user.Id);
        return new Token(newJwtToken, newRefreshToken, DateTime.UtcNow, DateTime.UtcNow.AddMinutes(GetJwtExpirationMinutes()));
    }

    public async Task<bool> ValidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);
        var isValid = storedToken?.IsActive == true;
        
        _logger.LogInformation("Refresh token validation result: {IsValid}", isValid);
        return isValid;
    }

    public async Task<User?> GetUserFromRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);
        if (storedToken?.IsActive != true)
        {
            _logger.LogWarning("Inactive refresh token used to get user");
            return null;
        }

        var user = await _userRepository.FindAsync(storedToken.UserId, cancellationToken);
        _logger.LogInformation("Retrieved user {UserId} from refresh token", user?.Id);
        return user;
    }

    public async Task<RefreshTokenResponse> StoreRefreshToken(int userId, string refreshToken, CancellationToken cancellationToken = default)
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
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Refresh token stored for user {UserId}", userId);
        return _mapper.Map<RefreshTokenResponse>(refreshTokenEntity);
    }

    public async Task<RefreshTokenResponse> RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);
        if (storedToken == null)
        {
            _logger.LogWarning("Attempted to revoke non-existent refresh token");
            throw new TokenValidationException("Refresh token not found");
        }

        storedToken.IsRevoked = true;
        await _refreshTokenRepository.UpdateAsync(storedToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Refresh token revoked for user {UserId}", storedToken.UserId);
        return _mapper.Map<RefreshTokenResponse>(storedToken);
    }

    public async Task<List<RefreshTokenResponse>> RevokeAllUserTokensAsync(int userId, CancellationToken cancellationToken = default)
    {
        var tokens = await _refreshTokenRepository.GetByUserIdAsync(userId, cancellationToken);
        var activeTokens = tokens.Where(t => t.IsActive).ToList();

        foreach (var token in activeTokens)
        {
            token.IsRevoked = true;
            await _refreshTokenRepository.UpdateAsync(token, cancellationToken);
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Revoked {Count} active tokens for user {UserId}", activeTokens.Count, userId);
        return _mapper.Map<List<RefreshTokenResponse>>(activeTokens);
    }

    public async Task CleanupExpiredTokensAsync(CancellationToken cancellationToken = default)
    {
        await _refreshTokenRepository.CleanupExpiredTokensAsync(cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Expired tokens cleanup completed");
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
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
            return principal;
        }
        catch (Exception)
        {
            throw new TokenValidationException("Invalid token format");
        }
    }

    private static string GenerateRefreshToken()
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