using Microsoft.EntityFrameworkCore;
using Petsgram.Application.Interfaces.PetPhotos;
using Petsgram.Domain.Entities;
using Petsgram.Infrastructure.DbContexts;

namespace Petsgram.Infrastructure.Repositories;

public class PetPhotoRepository : IPetPhotoRepository
{
    private readonly PetsgramDbContext _dbContext;

    public PetPhotoRepository(PetsgramDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<PetPhoto>> GetAllAsync(int petId)
    {
        return await _dbContext.PetPhotos
            .AsNoTracking()
            .Where(pp => pp.PetId == petId)
            .ToArrayAsync();
    }

    public async Task<PetPhoto?> FindAsync(int id)
    {
        return await _dbContext.PetPhotos.FindAsync(id);
    }

    public async Task<PetPhoto> AddAsync(PetPhoto entity)
    {
        await _dbContext.PetPhotos.AddAsync(entity);
        return entity;
    }

    public async Task RemoveAsync(int id)
    {
        var photo = await FindAsync(id);
        if (photo == null)
            throw new ArgumentException($"PetPhoto with id:{id} not found");

        _dbContext.PetPhotos.Remove(photo);
    }

    public async Task UpdateAsync(PetPhoto entity)
    {
        await _dbContext.PetPhotos
            .Where(pp => pp.Id == entity.Id)
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(pp => pp.Path, entity.Path)
                .SetProperty(pp => pp.PublicUrl, entity.PublicUrl)
                .SetProperty(pp => pp.PetId, entity.PetId)
            );
    }
}