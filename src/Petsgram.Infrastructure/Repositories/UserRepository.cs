using Petsgram.Application.DTOs.Users;
using Petsgram.Application.Interfaces.Users;
using Petsgram.Domain.Entities;

namespace Petsgram.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    public Task AddUserAsync(AddUserDto userRequest)
    {
        throw new NotImplementedException();
    }

    public Task<ICollection<Pet>> GetPetsByUserIdAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public Task<User> GetUserByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ICollection<User>> GetUsersAsync(int count, int skip)
    {
        throw new NotImplementedException();
    }
}
