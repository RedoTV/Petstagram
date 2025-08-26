using Petsgram.Application.Settings;

namespace Petsgram.WebAPI.Extensions.ServiceCollectionExtensions;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppSettings(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.Configure<StorageSettings>(
            configuration.GetSection(StorageSettings.SectionName)
        );
    
        services.Configure<AuthSettings>(
            configuration.GetSection(AuthSettings.SectionName)
        );
    
        services.Configure<ExchangeRateSettings>(
            configuration.GetSection(ExchangeRateSettings.SectionName)
        );
    
        services.Configure<AdminSettings>(
            configuration.GetSection(AdminSettings.SectionName)
        );
        
        return services;
    }
}