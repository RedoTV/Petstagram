using Microsoft.EntityFrameworkCore;
using Petsgram.Application.Interfaces.PetTypes;
using Petsgram.Domain.Entities;
using Petsgram.Infrastructure.DbContexts;

namespace Petsgram.Infrastructure.Repositories;

public class PetTypeRepository : IPetTypeRepository
{
    private readonly PetsgramDbContext _dbContext;

    public PetTypeRepository(PetsgramDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<PetType>> GetAllAsync()
    {
        return await _dbContext.PetTypes
            .AsNoTracking()
            .ToArrayAsync();
    }

    public async Task<PetType?> FindAsync(int id)
    {
        return await _dbContext.PetTypes.FindAsync(id);
    }

    public async Task<PetType> AddAsync(PetType entity)
    {
        await _dbContext.PetTypes.AddAsync(entity);
        return entity;
    }

    public async Task RemoveAsync(int id)
    {
        var type = await FindAsync(id);
        if (type == null)
            throw new ArgumentException($"PetType with id:{id} not found");

        _dbContext.PetTypes.Remove(type);
    }

    public async Task UpdateAsync(PetType entity)
    {
        await _dbContext.PetTypes
            .Where(pt => pt.Id == entity.Id)
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(pt => pt.Name, entity.Name)
            );
    }
}