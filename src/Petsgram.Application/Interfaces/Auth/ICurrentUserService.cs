using Petsgram.Domain.Entities;

namespace Petsgram.Application.Interfaces.Auth;

public interface ICurrentUserService
{
    int? GetCurrentUserId();
    string? GetCurrentUserName();
    string? GetCurrentUserRole();
    Task<User?> GetCurrentUserAsync(CancellationToken cancellationToken = default);
}