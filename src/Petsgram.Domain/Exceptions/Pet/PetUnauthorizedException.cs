namespace Petsgram.Domain.Exceptions.Pet;

public class PetUnauthorizedException(string message) : 
    Exception(message);