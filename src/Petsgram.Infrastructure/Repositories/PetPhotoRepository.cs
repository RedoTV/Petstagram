using Petsgram.Application.Interfaces.PetPhotos;
using Petsgram.Domain.Entities;
using Petsgram.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Petsgram.Infrastructure.Repositories;

public class PetPhotoRepository : IPetPhotoRepository
{
    private readonly PetsgramDbContext _context;

    public PetPhotoRepository(PetsgramDbContext context)
    {
        _context = context;
    }

    public Task<List<PetPhoto>> GetAllAsync(int petId, CancellationToken cancellationToken = default)
    {
        return _context.PetPhotos.Where(pp => pp.PetId == petId).ToListAsync(cancellationToken);
    }

    public Task<PetPhoto?> FindAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.PetPhotos.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<PetPhoto> AddAsync(PetPhoto entity, CancellationToken cancellationToken = default)
    {
        await _context.PetPhotos.AddAsync(entity, cancellationToken);
        return entity;
    }

    public Task UpdateAsync(PetPhoto entity, CancellationToken cancellationToken = default)
    {
        _context.PetPhotos.Update(entity);
        return Task.CompletedTask;
    }

    public async Task RemoveAsync(int id, CancellationToken cancellationToken = default)
    {
        await _context.PetPhotos.Where(x => x.Id == id).ExecuteDeleteAsync(cancellationToken);
    }
}
