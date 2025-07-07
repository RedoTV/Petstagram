using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DbContexts;

public class PetsgramDbContext : DbContext
{
    public PetsgramDbContext(DbContextOptions<PetsgramDbContext> opt) : base(opt) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Pet> Pets { get; set; }
}
