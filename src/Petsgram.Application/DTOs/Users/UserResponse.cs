using Petsgram.Application.DTOs.Pets;

namespace Petsgram.Application.DTOs.Users;

public class UserResponse
{
    public int Id { get; set; }
    public required string UserName { get; set; }

    public List<PetResponse>? Pets { get; set; } = [];
}
