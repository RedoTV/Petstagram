using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Petsgram.Infrastructure.DbContexts;

namespace Petsgram.Infrastructure.Extensions;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddSqlServerDatabase(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddDbContext<PetsgramDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DbConnection"),
                b => b.MigrationsAssembly("Petsgram.Infrastructure")));
        
        return services;
    }
}