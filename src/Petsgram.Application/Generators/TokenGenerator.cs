using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Petsgram.Application.Settings;
using Petsgram.Domain.Entities;

namespace Petsgram.Application.Generators;

public class TokenGenerator : ITokenGenerator
{
    private readonly AuthSettings _authSettings;

    public TokenGenerator(IOptions<AuthSettings> authSettings)
    {
        _authSettings = authSettings.Value;
    }

    public Token GenerateToken(User user)
    {
        var claims = GetClaims(user);
        var symmetricKey = _authSettings.GetSymmetricSecurityKey();
        var signingCredentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256Signature);

        var createdAt = DateTime.UtcNow;
        var expiresAt = createdAt.AddDays(int.Parse(_authSettings.AccessTokenExpireDays));

        var jwt = new JwtSecurityToken(
            issuer: _authSettings.Issuer,
            audience: _authSettings.Audience,
            claims: claims,
            notBefore: createdAt,
            expires: expiresAt,
            signingCredentials: signingCredentials
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);
        var refreshToken = GenerateRefreshToken();

        return new Token(accessToken, refreshToken, createdAt, expiresAt);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private List<Claim> GetClaims(User user)
    {
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
           
            //Jwt token identifier 
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        return claims;
    }
}
