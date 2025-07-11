using Petsgram.Application.DTOs.Users;
using Petsgram.Domain.Entities;

namespace Petsgram.Application.Interfaces.Users;

public interface IUserRepository
{
    Task<ICollection<User>> GetUsersAsync(int count, int skip);
    Task<User> GetUserByIdAsyncAsync(int id);
    Task<ICollection<Pet>> GetPetsByUserIdAsync(int userId);
    Task AddUserAsync(AddUserDto userRequest);
}