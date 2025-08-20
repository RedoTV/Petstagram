using Microsoft.Extensions.DependencyInjection;
using Petsgram.Application.Interfaces.Auth;
using Petsgram.Application.Interfaces.PetPhotos;
using Petsgram.Application.Interfaces.Pets;
using Petsgram.Application.Interfaces.PetTypes;
using Petsgram.Application.Interfaces.UnitOfWork;
using Petsgram.Application.Interfaces.Users;
using Petsgram.Infrastructure.Repositories;
using Petsgram.Infrastructure.Services.Auth;
using Petsgram.Infrastructure.UnitOfWork;

namespace Petsgram.Infrastructure.Extensions;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IPetRepository, PetRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPetPhotoRepository, PetPhotoRepository>();
        services.AddScoped<IPetTypeRepository, PetTypeRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWorkImplementation>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        
        return services;
    }
}