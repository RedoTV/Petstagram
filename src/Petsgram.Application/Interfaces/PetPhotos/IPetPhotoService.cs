using Petsgram.Application.DTOs.PetPhotos;

namespace Petsgram.Application.Interfaces.PetPhotos;

public interface IPetPhotoService
{
    Task<List<PetPhotoResponse>> GetAllByPetIdAsync(int petId, CancellationToken cancellationToken = default);
    Task<PetPhotoResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddPhotoAsync(int petId, int userId, Stream fileStream, string fileName, CancellationToken cancellationToken = default);
    Task RemovePhotoAsync(int id, CancellationToken cancellationToken = default);
}