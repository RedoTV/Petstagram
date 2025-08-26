namespace Petsgram.Domain.Exceptions.PetType;

public class PetTypeValidationException(string message) : 
    Exception(message);
