using Petsgram.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Petsgram.Infrastructure.EntityConfigurations;
using System.Reflection;

namespace Petsgram.Infrastructure.DbContexts;

public class PetsgramDbContext : DbContext
{
    public PetsgramDbContext(DbContextOptions<PetsgramDbContext> opt) : base(opt) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Pet> Pets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
