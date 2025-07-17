using Petsgram.Application.DTOs.Users;

namespace Petsgram.Application.Interfaces.Users;

public interface IUserService
{
    Task<IEnumerable<UserResponse>> GetAllAsync(int count, int skip);
    Task<UserResponse> GetByIdAsync(int id);
    Task AddUserAsync(CreateUserDto user);
    Task UpdateUserAsync(int id, CreateUserDto user);
    Task RemoveUserAsync(int id);
}
