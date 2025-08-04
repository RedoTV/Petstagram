using Petsgram.Application.Interfaces.Pets;
using Petsgram.Application.Interfaces.Users;
using Petsgram.Application.Interfaces.PetPhotos;
using Petsgram.Application.Interfaces.PetTypes;
using Petsgram.Application.Mapper;
using Petsgram.Application.Services.Pets;
using Petsgram.Application.Services.Users;
using Petsgram.Application.Services.PetPhotos;
using Petsgram.Application.Services.PetTypes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Petsgram.Application.Settings;
using Petsgram.Application.Generators;

namespace Petsgram.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IPetService, PetService>();
        services.AddScoped<IPetPhotoService, PetPhotoService>();
        services.AddScoped<IPetTypeService, PetTypeService>();

        services.AddScoped<ITokenGenerator, TokenGenerator>();

        services.AddAutoMapper(
            typeof(UserProfile),
            typeof(PetProfile),
            typeof(PetPhotoProfile),
            typeof(PetTypeProfile)
        );

        return services;
    }
}