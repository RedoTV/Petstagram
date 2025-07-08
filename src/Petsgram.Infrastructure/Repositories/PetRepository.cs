using Petsgram.Application.Interfaces.Pets;
using Petsgram.Domain.Entities;
using Petsgram.Infrastructure.DbContexts;

namespace Petsgram.Infrastructure.Repositories;

public class PetRepository : IPetRepository
{
    private readonly PetsgramDbContext _dbContext;

    public PetRepository(PetsgramDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ICollection<Pet>> GetAsync(int userId)
    {
        User? user = await _dbContext.Users.FindAsync(userId);

        if (user is null || user.Pets is null)
            return [];

        return user.Pets;
    }

    public async Task<Pet> AddAsync(Pet pet)
    {
        await _dbContext.Pets.AddAsync(pet);
        await _dbContext.SaveChangesAsync();

        return pet;
    }

    public async Task RemoveAsync(int petId)
    {
        Pet? pet = await _dbContext.Pets.FindAsync(petId);

        if (pet is not null)
        {
            _dbContext.Pets.Remove(pet);
            await _dbContext.SaveChangesAsync();
        }
    }
}
