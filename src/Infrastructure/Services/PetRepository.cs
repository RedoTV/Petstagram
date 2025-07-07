using Application.Interfaces.Pets;
using Domain.Entities;
using Infrastructure.DbContexts;

namespace Infrastructure.Services;

public class PetRepository : IPetRepository
{
    private readonly PetsgramDbContext _dbContext;

    public PetRepository(PetsgramDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Pet> AddPetToUserAsync(Pet pet)
    {
        await _dbContext.Pets.AddAsync(pet);
        await _dbContext.SaveChangesAsync();

        return pet;
    }

    public async Task<ICollection<Pet>> GetUserPetsByIdAsync(int userId)
    {
        User? user = await _dbContext.Users.FindAsync(userId);

        if (user is null || user.Pets is null)
            return [];

        return user.Pets;
    }

    public async Task RemoveUserPetAsync(int petId)
    {
        Pet? pet = await _dbContext.Pets.FindAsync(petId);

        if (pet is not null)
        {
            _dbContext.Pets.Remove(pet);
            await _dbContext.SaveChangesAsync();
        }
    }
}
