using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Petsgram.Application.Interfaces.Auth;
using Petsgram.Application.Interfaces.Caching;
using Petsgram.Application.Interfaces.PetPhotos;
using Petsgram.Application.Interfaces.Pets;
using Petsgram.Application.Interfaces.PetTypes;
using Petsgram.Application.Interfaces.UnitOfWork;
using Petsgram.Application.Interfaces.Users;
using Petsgram.Application.Settings;
using Petsgram.Infrastructure.Repositories;
using Petsgram.Infrastructure.Services.Auth;
using Petsgram.Infrastructure.Services.RedisCache;
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
        
        services.AddSingleton<ICacheService>(sp =>
        {
            var opt = sp.GetRequiredService<IOptions<RedisSettings>>().Value;
            var defaultExpiration = TimeSpan.FromSeconds(opt.DefaultTtlSeconds);
            return new RedisCacheService(
                sp.GetRequiredService<IDistributedCache>(),
                defaultExpiration
            );
        });
        
        return services;
    }
}