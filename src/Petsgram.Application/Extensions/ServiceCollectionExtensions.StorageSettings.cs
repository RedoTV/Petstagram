using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Petsgram.Application.Settings;

namespace Petsgram.Application.Extensions;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddStorageSettings(this IServiceCollection services)
    {
        services.Configure<StorageSettings>(
            services.BuildServiceProvider().GetRequiredService<IConfiguration>().GetSection("Storage"));

        return services;
    }
}