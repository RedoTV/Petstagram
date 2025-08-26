using Petsgram.Domain.Enums;

namespace Petsgram.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public required string UserName { get; set; }
    public required string HashedPassword { get; set; }
    public AuthRoles Role { get; set; }

    public List<Pet> Pets { get; set; } = [];
    public List<RefreshToken> RefreshTokens { get; set; } = [];
}
