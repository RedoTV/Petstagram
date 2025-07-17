using Microsoft.EntityFrameworkCore;
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

    public async Task<IEnumerable<Pet>> GetAllAsync(int userId)
    {
        return await _dbContext.Pets
            .AsNoTracking()
            .Include(p => p.PetType)
            .Include(p => p.Photos)
            .Where(p => p.UserId == userId)
            .ToArrayAsync();
    }

    public async Task<Pet?> FindAsync(int petId)
    {
        return await _dbContext.Pets
            .AsNoTracking()
            .Include(p => p.PetType)
            .Include(p => p.Photos)
            .FirstOrDefaultAsync(p => p.Id == petId);
    }

    public async Task<Pet> AddAsync(Pet pet)
    {
        await _dbContext.Pets.AddAsync(pet);
        return pet;
    }

    public async Task RemoveAsync(int petId)
    {
        var pet = await FindAsync(petId);
        if (pet == null)
            throw new ArgumentException($"Pet with id:{petId} not found");

        _dbContext.Pets.Remove(pet);
    }

    public async Task UpdateAsync(Pet pet)
    {
        await _dbContext.Pets
            .Where(p => p.Id == pet.Id)
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(p => p.PetName, pet.PetName)
                .SetProperty(p => p.PetTypeId, pet.PetTypeId)
                .SetProperty(p => p.UserId, pet.UserId)
            );
    }
}
