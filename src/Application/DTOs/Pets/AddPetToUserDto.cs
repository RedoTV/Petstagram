namespace Application.DTOs.Pets;

public class AddPetToUserDto
{
    public required string PetName { get; set; } = null!;
    public string PetType { get; set; } = null!;
}
