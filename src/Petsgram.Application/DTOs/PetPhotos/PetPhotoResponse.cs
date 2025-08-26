namespace Petsgram.Application.DTOs.PetPhotos;

public class PetPhotoResponse
{
    public int Id { get; set; }
    public required string PublicUrl { get; set; }
}