using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Petsgram.Application.Extensions;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        
        return services;
    }
}