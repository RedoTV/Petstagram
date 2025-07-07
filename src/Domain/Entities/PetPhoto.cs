namespace Domain.Entities;

public class PetPhoto
{
    public int Id { get; set; }
    public required string Path { get; set; } = null!;
    public string PublicUrl { get; set; } = null!;

    public int PetId { get; set; }
    public Pet Pet { get; set; } = null!;
}