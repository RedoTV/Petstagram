using Petsgram.Application.Interfaces.Shared;
using Petsgram.Domain.Entities;

namespace Petsgram.Application.Interfaces.Users;

public interface IUserRepository : IGenericRepository<User>
{
    Task<IEnumerable<User>> GetAllAsync(int count, int skip);
    Task<User?> GetByUserNameAsync(string userName);
    Task<bool> UserNameExistsAsync(string userName);
}