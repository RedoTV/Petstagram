using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Petsgram.Application.Settings;

namespace Petsgram.Infrastructure.Extensions;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddRedisCaching(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<RedisSettings>(
            configuration.GetSection(RedisSettings.SectionName)
        );
        
        var settings = configuration
            .GetSection(RedisSettings.SectionName)
            .Get<RedisSettings>() ?? new RedisSettings();

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = settings.ConnectionString;
            options.InstanceName  = settings.KeyPrefix;
        });

        return services;
    }
}