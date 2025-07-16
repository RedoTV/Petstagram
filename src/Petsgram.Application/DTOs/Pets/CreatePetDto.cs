namespace Petsgram.Application.DTOs.Pets;

public class CreatePetDto
{
    public required string PetName { get; set; }
    public required string PetType { get; set; }
}