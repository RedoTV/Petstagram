using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Petsgram.Application.Interfaces.Auth;
using Petsgram.Application.Settings;
using Petsgram.Domain.Entities;

namespace Petsgram.Infrastructure.Services.Auth;

public class AuthService : IAuthService
{
    private readonly AuthSettings _authSettings;

    public AuthService(IOptions<AuthSettings> authSettings)
    {
        _authSettings = authSettings.Value;
    }
    public string GenerateToken(User user)
    {
        var claims = GetClaims(user);

        var symmetricKey = _authSettings.GetSymmetricSecurityKey();
        var signingCredentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256Signature);

        var expires = DateTime.Now.AddDays(int.Parse(_authSettings.AccessTokenExpireDays));

        var jwt = new JwtSecurityToken(
            issuer: _authSettings.Issuer,
            audience: _authSettings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: signingCredentials
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);
        return accessToken;
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
