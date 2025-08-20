namespace Petsgram.Domain.Exceptions.PetPhoto;

public class PetPhotoUnauthorizedException(string message) : 
    Exception(message);