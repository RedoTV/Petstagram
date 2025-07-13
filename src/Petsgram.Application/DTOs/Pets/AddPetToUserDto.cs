namespace Petsgram.Application.DTOs.Pets;

public class AddPetToUserDto
{
    public required string PetName { get; set; }
    public required string PetType { get; set; }
}
