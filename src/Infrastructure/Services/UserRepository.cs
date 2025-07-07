using Application.DTOs.Users;
using Application.Interfaces.Users;
using Domain.Entities;

namespace Infrastructure.Services;

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
