namespace Petsgram.Domain.Exceptions.PetPhoto;

public class PetPhotoNotFoundException(int id) : 
    Exception($"Pet photo with id {id} not found");
