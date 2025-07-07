using Application.DTOs.Users;
using Domain.Entities;

namespace Application.Interfaces.Users;

public interface IUserRepository
{
    public Task<ICollection<User>> GetUsersAsync(int count, int skip);
    public Task<User> GetUserByIdAsync(int id);
    public Task<ICollection<Pet>> GetPetsByUserIdAsync(int userId);
    public Task AddUserAsync(AddUserDto userRequest);
}