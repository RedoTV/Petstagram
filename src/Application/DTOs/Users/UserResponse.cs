using Application.DTOs.Pets;

namespace Application.DTOs.Users;

public class UserResponse
{
    public int Id { get; set; }
    public required string UserName { get; set; } = null!;

    public ICollection<PetResponse>? Pets { get; set; }
}
