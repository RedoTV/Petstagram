using Petsgram.Application.Interfaces.Pets;
using Petsgram.Application.Interfaces.Users;
using Petsgram.Application.Mapper;
using Petsgram.Application.Services.Pets;
using Petsgram.Application.Services.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Petsgram.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IPetService, PetService>();

        services.AddAutoMapper(typeof(UserProfile), typeof(PetProfile));

        return services;
    }
}
