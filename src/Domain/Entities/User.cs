namespace Domain.Entities;

public class User
{
    public int Id { get; set; }
    public required string UserName { get; set; } = null!;

    public ICollection<Pet>? Pets { get; set; }
}
