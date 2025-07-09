using Petsgram.Application.DTOs.Users;

namespace Petsgram.Application.Interfaces.Users;

public interface IUserService
{
    Task<ICollection<UserResponse>> GetUsersWithPetsAsync(int count, int skip);
    Task<UserResponse> GetUserByIdAsync(int id);
}
