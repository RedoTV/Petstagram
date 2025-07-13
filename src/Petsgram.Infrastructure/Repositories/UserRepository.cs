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
            .Take(count)
            .Skip(skip)
            .ToListAsync();
    }

    public async Task<User?> FindAsync(int id)
    {
        //without error handling 
        return await _dbContext.Users.FindAsync(id);
    }

    public async Task<User> AddAsync(User user)
    {
        await _dbContext.Users.AddAsync(user);
        return user;
    }

    public async Task RemoveAsync(int userId)
    {
        //with error handling
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
                setter.SetProperty(u => u.UserName, user.UserName
            ));
    }
}
