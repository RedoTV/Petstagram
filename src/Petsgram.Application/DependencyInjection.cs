using Petsgram.Application.Interfaces.Pets;
using Petsgram.Application.Interfaces.Users;
using Petsgram.Application.Interfaces.PetPhotos;
using Petsgram.Application.Interfaces.PetTypes;
using Petsgram.Application.Services.Pets;
using Petsgram.Application.Services.Users;
using Petsgram.Application.Services.PetPhotos;
using Petsgram.Application.Services.PetTypes;
using Petsgram.Application.Generators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Petsgram.Application.Settings;
using Petsgram.Application.Mapper;

namespace Petsgram.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPetService, PetService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IPetPhotoService, PetPhotoService>();
        services.AddScoped<IPetTypeService, PetTypeService>();
        services.AddScoped<ITokenGenerator, TokenGenerator>();

        services.AddAutoMapper(
            typeof(UserProfile).Assembly,
            typeof(PetProfile).Assembly,
            typeof(PetTypeProfile).Assembly,
            typeof(PetPhotoProfile).Assembly);

        services.Configure<StorageSettings>(
            services.BuildServiceProvider().GetRequiredService<IConfiguration>().GetSection("Storage"));

        return services;
    }
}