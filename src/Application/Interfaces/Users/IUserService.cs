using Application.DTOs.Users;

namespace Application.Interfaces.Users;

public interface IUserService
{
    Task<ICollection<UserResponse>> GetUsersWithPets(int count, int skip);
    Task<UserResponse> GetUserById(int id);
}
