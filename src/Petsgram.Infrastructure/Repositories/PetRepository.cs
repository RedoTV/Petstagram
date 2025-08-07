using Petsgram.Application.Interfaces.Pets;
using Petsgram.Domain.Entities;
using Petsgram.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Petsgram.Infrastructure.Repositories;

public class PetRepository : IPetRepository
{
    private readonly PetsgramDbContext _context;

    public PetRepository(PetsgramDbContext context)
    {
        _context = context;
    }

    public Task<List<Pet>> GetAllAsync(int userId, CancellationToken cancellationToken = default)
    {
        return _context.Pets.Where(p => p.UserId == userId).ToListAsync(cancellationToken);
    }

    public Task<Pet?> FindAsync(int petId, CancellationToken cancellationToken = default)
    {
        return _context.Pets.FirstOrDefaultAsync(x => x.Id == petId, cancellationToken);
    }

    public async Task<Pet> AddAsync(Pet pet, CancellationToken cancellationToken = default)
    {
        await _context.Pets.AddAsync(pet, cancellationToken);
        return pet;
    }

    public Task UpdateAsync(Pet entity, CancellationToken cancellationToken = default)
    {
        _context.Pets.Update(entity);
        return Task.CompletedTask;
    }

    public async Task RemoveAsync(int id, CancellationToken cancellationToken = default)
    {
        await _context.Pets.Where(x => x.Id == id).ExecuteDeleteAsync(cancellationToken);
    }
}
