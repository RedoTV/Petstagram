using Petsgram.Application.Extensions;

namespace Petsgram.WebAPI.Extensions.ServiceCollectionExtensions;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationLayer(
        this IServiceCollection services)
    {
        services
            .AddApplicationServices()
            .AddAutoMapper();
        
        return services;
    }
}