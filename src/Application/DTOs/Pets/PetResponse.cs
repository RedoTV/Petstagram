namespace Application.DTOs.Pets;

public class PetResponse
{
    public int Id { get; set; }
    public required string PetName { get; set; } = null!;
    public string PetType { get; set; } = null!;
    public ICollection<string> PublicUrls { get; set; } = null!;
}
