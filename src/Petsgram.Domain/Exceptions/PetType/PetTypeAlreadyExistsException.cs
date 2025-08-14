namespace Petsgram.Domain.Exceptions.PetType;

public class PetTypeAlreadyExistsException(string name) : 
    Exception($"Pet type '{name}' already exists");