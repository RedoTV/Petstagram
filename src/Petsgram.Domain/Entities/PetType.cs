namespace Petsgram.Domain.Entities;

public class PetType
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<Pet> Pets { get; set; } = [];
}
