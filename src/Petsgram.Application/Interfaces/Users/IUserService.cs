using Petsgram.Application.DTOs.Users;

namespace Petsgram.Application.Interfaces.Users;

public interface IUserService
{
    Task<List<UserResponse>> GetAllAsync(int count, int skip, CancellationToken cancellationToken = default);
    Task<UserResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<AuthResponse> RegisterAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> RefreshTokenAsync(string accessToken, string refreshToken, CancellationToken cancellationToken = default);
    Task<UserResponse> RemoveUserAsync(int id, CancellationToken cancellationToken = default);
}