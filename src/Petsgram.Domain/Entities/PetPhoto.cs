namespace Petsgram.Domain.Entities;

public class PetPhoto
{
    public int Id { get; set; }
    public required string Path { get; set; }
    public required string PublicUrl { get; set; }

    public int PetId { get; set; }
    public Pet Pet { get; set; } = null!;
}