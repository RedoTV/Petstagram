using Petsgram.Application.DTOs.Users;

namespace Petsgram.Application.Interfaces.Users;

public interface IUserService
{
    Task<List<UserResponse>> GetAllAsync(int count, int skip, CancellationToken cancellationToken = default);
    Task<UserResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> RegisterAsync(CreateUserDto userDto, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> RefreshTokenAsync(string accessToken, string refreshToken, CancellationToken cancellationToken = default);
    Task RemoveUserAsync(int id);
}
