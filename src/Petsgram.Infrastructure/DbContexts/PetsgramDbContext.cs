using Petsgram.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Petsgram.Infrastructure.DbContexts;

public class PetsgramDbContext : DbContext
{
    public PetsgramDbContext(DbContextOptions<PetsgramDbContext> opt) : base(opt) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Pet> Pets { get; set; }
    public DbSet<PetPhoto> PetPhotos { get; set; }
    public DbSet<PetType> PetTypes { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
