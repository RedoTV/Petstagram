namespace Petsgram.Domain.Exceptions.Pet;

public class PetNotFoundException(int id) : 
    Exception($"Pet with id {id} not found");