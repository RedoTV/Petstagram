using Petsgram.Domain.Entities;

namespace Petsgram.Application.Generators;

public interface ITokenGenerator
{
    Token GenerateToken(User user);
}
