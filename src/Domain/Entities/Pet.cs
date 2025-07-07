namespace Domain.Entities;

public class Pet
{
    public int Id { get; set; }
    public required string PetName { get; set; } = null!;
    public PetType PetType { get; set; } = null!;
    public ICollection<PetPhoto> Photos { get; set; } = new List<PetPhoto>();

    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
