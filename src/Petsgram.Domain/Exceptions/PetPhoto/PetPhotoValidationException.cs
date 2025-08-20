namespace Petsgram.Domain.Exceptions.PetPhoto;

public class PetPhotoValidationException(string message) : 
    Exception(message);