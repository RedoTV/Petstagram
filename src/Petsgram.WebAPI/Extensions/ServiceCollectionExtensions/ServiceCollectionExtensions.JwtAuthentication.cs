using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Petsgram.Application.Settings;

namespace Petsgram.WebAPI.Extensions.ServiceCollectionExtensions;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var authSettings = configuration.GetSection(AuthSettings.SectionName).Get<AuthSettings>();
                if (authSettings == null ||
                    string.IsNullOrEmpty(authSettings.SecretKey) ||
                    string.IsNullOrEmpty(authSettings.Issuer) ||
                    string.IsNullOrEmpty(authSettings.Audience))
                    throw new InvalidOperationException(JwtBearerDefaults.AuthenticationScheme);

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = authSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = authSettings.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = authSettings.GetSymmetricSecurityKey(),
                    ValidateLifetime = true,
                };
            });
        
        return services;
    }
}