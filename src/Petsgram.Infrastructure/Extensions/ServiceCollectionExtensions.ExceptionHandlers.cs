using Microsoft.Extensions.DependencyInjection;
using Petsgram.Infrastructure.ExceptionHandlers;

namespace Petsgram.Infrastructure.Extensions;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddExceptionHandlers(this IServiceCollection services)
    {
        services.AddExceptionHandler<PetTypeExceptionHandler>();
        services.AddExceptionHandler<PetExceptionHandler>();
        services.AddExceptionHandler<PetPhotoExceptionHandler>();
        services.AddExceptionHandler<UserExceptionHandler>();
        services.AddExceptionHandler<AuthExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        
        services.AddProblemDetails();
        
        return services;
    }
}