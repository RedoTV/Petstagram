using Petsgram.Application.Interfaces.Users;
using Petsgram.Domain.Entities;
using Petsgram.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Petsgram.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly PetsgramDbContext _context;

    public UserRepository(PetsgramDbContext context)
    {
        _context = context;
    }

    public Task<List<User>> GetAllAsync(int count, int skip, CancellationToken cancellationToken = default)
    {
        return _context.Users.Skip(skip).Take(count).ToListAsync(cancellationToken);
    }

    public Task<User?> FindAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<User> AddAsync(User entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var result = await _context.Users.AddAsync(entity, cancellationToken);
        return result.Entity;
    }

    public Task UpdateAsync(User entity, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(entity);
        return Task.CompletedTask;
    }

    public async Task RemoveAsync(int id, CancellationToken cancellationToken = default)
    {
        await _context.Users.Where(x => x.Id == id).ExecuteDeleteAsync(cancellationToken);
    }

    public Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        return _context.Users.FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);
    }

    public Task<bool> UserNameExistsAsync(string userName, CancellationToken cancellationToken = default)
    {
        return _context.Users.AnyAsync(u => u.UserName == userName, cancellationToken);
    }
}
