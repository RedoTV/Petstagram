namespace Petsgram.Domain.Entities;

public class Pet
{
    public int Id { get; set; }
    public required string PetName { get; set; }
    public List<PetPhoto> Photos { get; set; } = [];

    public int PetTypeId { get; set; }
    public PetType PetType { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
