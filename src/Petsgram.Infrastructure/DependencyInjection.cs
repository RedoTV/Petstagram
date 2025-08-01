using Petsgram.Application.Interfaces.Pets;
using Petsgram.Application.Interfaces.Users;
using Petsgram.Application.Interfaces.PetPhotos;
using Petsgram.Application.Interfaces.PetTypes;
using Petsgram.Application.Interfaces.UnitOfWork;
using Petsgram.Infrastructure.DbContexts;
using Petsgram.Infrastructure.Repositories;
using Petsgram.Infrastructure.UnitOfWork;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UnitOfWorkImpl = Petsgram.Infrastructure.UnitOfWork.UnitOfWork;
using Petsgram.Application.Interfaces.Auth;
using Petsgram.Infrastructure.Services.Auth;

namespace Petsgram.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSqlServer<PetsgramDbContext>(configuration.GetConnectionString("DbConnection"));

        services.AddScoped<IPetRepository, PetRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPetPhotoRepository, PetPhotoRepository>();
        services.AddScoped<IPetTypeRepository, PetTypeRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWorkImpl>();

        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}