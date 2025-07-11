namespace Petsgram.Application.DTOs.Pets;

public class PetResponse
{
    public int Id { get; set; }
    public required string PetName { get; set; }
    public required string PetType { get; set; }
    public required List<string> PublicUrls { get; set; } = [];
}
