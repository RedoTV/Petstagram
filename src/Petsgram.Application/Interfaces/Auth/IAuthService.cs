using Petsgram.Domain.Entities;

namespace Petsgram.Application.Interfaces.Auth;

public interface IAuthService
{
    string GenerateToken(User user);
}
