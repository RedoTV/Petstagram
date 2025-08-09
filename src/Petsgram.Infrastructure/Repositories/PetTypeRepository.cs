using Petsgram.Application.Interfaces.PetTypes;
using Petsgram.Domain.Entities;
using Petsgram.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Petsgram.Infrastructure.Repositories;

public class PetTypeRepository : IPetTypeRepository
{
    private readonly PetsgramDbContext _context;

    public PetTypeRepository(PetsgramDbContext context)
    {
        _context = context;
    }

    public Task<List<PetType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _context.PetTypes.ToListAsync(cancellationToken);
    }

    public Task<PetType?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return _context.PetTypes.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }

    public Task<PetType?> FindAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.PetTypes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<PetType> AddAsync(PetType entity, CancellationToken cancellationToken = default)
    {
        await _context.PetTypes.AddAsync(entity, cancellationToken);
        return entity;
    }

    public Task UpdateAsync(PetType entity, CancellationToken cancellationToken = default)
    {
        _context.PetTypes.Update(entity);
        return Task.CompletedTask;
    }

    public async Task RemoveAsync(int id, CancellationToken cancellationToken = default)
    {
        await _context.PetTypes.Where(x => x.Id == id).ExecuteDeleteAsync(cancellationToken);
    }
}
