using Petsgram.Application.DTOs.Users;

namespace Petsgram.Application.Interfaces.Users;

public interface IUserService
{
    Task<IEnumerable<UserResponse>> GetAllAsync(int count, int skip);
    Task<UserResponse> GetByIdAsync(int id);
    Task<AuthResponseDto> RegisterAsync(CreateUserDto userDto);
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
    Task RemoveUserAsync(int id);
}
