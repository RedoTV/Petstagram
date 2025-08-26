using Microsoft.AspNetCore.Http;

namespace Petsgram.Application.DTOs.PetPhotos;

public class UploadPhotoRequest
{
    public int PetId { get; set; }
    public IFormFile File { get; set; } = null!;
}