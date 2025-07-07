using Application.Interfaces.Pets;
using Application.Interfaces.Users;
using Application.Mapper;
using Application.Services.Pets;
using Application.Services.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IPetService, PetService>();

        services.AddAutoMapper(typeof(UserProfile), typeof(PetProfile));

        return services;
    }
}
