using Microsoft.Extensions.DependencyInjection;
using Petsgram.Application.Generators;
using Petsgram.Application.Interfaces.PetPhotos;
using Petsgram.Application.Interfaces.Pets;
using Petsgram.Application.Interfaces.PetTypes;
using Petsgram.Application.Interfaces.Users;
using Petsgram.Application.Services.PetPhotos;
using Petsgram.Application.Services.Pets;
using Petsgram.Application.Services.PetTypes;
using Petsgram.Application.Services.Users;

namespace Petsgram.Application.Extensions;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IPetService, PetService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IPetPhotoService, PetPhotoService>();
        services.AddScoped<IPetTypeService, PetTypeService>();
        services.AddScoped<ITokenGenerator, TokenGenerator>();
        
        return services;
    }
}