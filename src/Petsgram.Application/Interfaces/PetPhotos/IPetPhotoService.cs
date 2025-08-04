using Petsgram.Application.DTOs.PetPhotos;

namespace Petsgram.Application.Interfaces.PetPhotos;

public interface IPetPhotoService
{
    Task<IEnumerable<PetPhotoResponse>> GetAllByPetIdAsync(int petId);
    Task<PetPhotoResponse> GetByIdAsync(int id);
    Task AddPhotoAsync(int petId, Stream fileStream, string fileName);
    Task RemovePhotoAsync(int id);
}