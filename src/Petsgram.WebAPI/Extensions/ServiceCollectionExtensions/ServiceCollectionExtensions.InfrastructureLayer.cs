using Petsgram.Infrastructure.Extensions;

namespace Petsgram.WebAPI.Extensions.ServiceCollectionExtensions;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureLayer(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services
            .AddExceptionHandlers()
            .AddSqlServerDatabase(configuration)
            .AddInfrastructureServices()
            .AddHttpContextAccessor();
        
        return services;
    }
}