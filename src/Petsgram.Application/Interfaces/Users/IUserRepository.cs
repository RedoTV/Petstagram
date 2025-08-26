using Petsgram.Application.Interfaces.Shared;
using Petsgram.Domain.Entities;

namespace Petsgram.Application.Interfaces.Users;

public interface IUserRepository : IGenericRepository<User>
{
    Task<List<User>> GetAllAsync(int count, int skip, CancellationToken cancellationToken = default);
    Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default);
    Task<bool> UserNameExistsAsync(string userName, CancellationToken cancellationToken = default);
}