using Petsgram.Application.DTOs.PetPhotos;

namespace Petsgram.Application.Interfaces.PetPhotos;

public interface IPetPhotoService
{
    Task<List<PetPhotoResponse>> GetAllByPetIdAsync(int petId, CancellationToken cancellationToken = default);
    Task<PetPhotoResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PetPhotoResponse> AddPhotoAsync(UploadPhotoRequest request, CancellationToken cancellationToken = default);
    Task<PetPhotoResponse> RemovePhotoAsync(int id, CancellationToken cancellationToken = default);
}