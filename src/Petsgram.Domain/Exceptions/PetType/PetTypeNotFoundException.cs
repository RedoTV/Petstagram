namespace Petsgram.Domain.Exceptions.PetType;

public class PetTypeNotFoundException(int id) : 
    Exception($"Pet type with id {id} not found");