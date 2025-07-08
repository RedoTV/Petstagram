using Petsgram.Application.DTOs.Users;

namespace Petsgram.Application.Interfaces.Users;

public interface IUserService
{
    Task<ICollection<UserResponse>> GetUsersWithPets(int count, int skip);
    Task<UserResponse> GetUserById(int id);
}
