namespace Petsgram.Domain.Exceptions.Pet;

public class PetValidationException(string message) : 
    Exception(message);