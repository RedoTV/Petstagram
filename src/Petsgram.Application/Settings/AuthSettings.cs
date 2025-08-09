using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Petsgram.Application.Settings;

public class AuthSettings
{
    public const string SectionName = "AuthSettings";
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string AccessTokenExpireDays { get; set; } = string.Empty;
    public string RefreshTokenExpireDays { get; set; } = string.Empty;

    public SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        if (string.IsNullOrEmpty(SecretKey))
            throw new InvalidOperationException("Secret key is empty");

        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
    }
}