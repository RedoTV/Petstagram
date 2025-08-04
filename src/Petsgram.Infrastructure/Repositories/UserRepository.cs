using Microsoft.EntityFrameworkCore;
using Petsgram.Application.DTOs.Users;
using Petsgram.Application.Interfaces.Users;
using Petsgram.Domain.Entities;
using Petsgram.Infrastructure.DbContexts;

namespace Petsgram.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly PetsgramDbContext _dbContext;

    public UserRepository(PetsgramDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<User>> GetAllAsync(int count, int skip)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .Include(u => u.Pets)
                .ThenInclude(p => p.PetType)
            .Include(u => u.Pets)
                .ThenInclude(p => p.Photos)
            .Skip(skip)
            .Take(count)
            .ToListAsync();
    }

    public async Task<User?> FindAsync(int id)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .Include(u => u.Pets)
                .ThenInclude(p => p.PetType)
            .Include(u => u.Pets)
                .ThenInclude(p => p.Photos)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByUserNameAsync(string userName)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .Include(u => u.Pets)
                .ThenInclude(p => p.PetType)
            .Include(u => u.Pets)
                .ThenInclude(p => p.Photos)
            .FirstOrDefaultAsync(u => u.UserName == userName);
    }

    public async Task<bool> UserNameExistsAsync(string userName)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .AnyAsync(u => u.UserName == userName);
    }

    public async Task<User> AddAsync(User user)
    {
        await _dbContext.Users.AddAsync(user);
        return user;
    }

    public async Task RemoveAsync(int userId)
    {
        var user = await FindAsync(userId);
        if (user == null)
            throw new ArgumentException($"User with id:{userId} not found");

        _dbContext.Users.Remove(user);
    }

    public async Task UpdateAsync(User user)
    {
        await _dbContext.Users
            .Where(u => u.Id == user.Id)
            .ExecuteUpdateAsync(setter =>
                setter.SetProperty(u => u.UserName, user.UserName)
            );
    }
}
