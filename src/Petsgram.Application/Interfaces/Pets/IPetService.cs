using Petsgram.Application.DTOs.Pets;

namespace Petsgram.Application.Interfaces.Pets;

public interface IPetService
{
    Task<List<PetResponse>> GetCurrentUserPetsAsync(CancellationToken cancellationToken = default);
    Task<List<PetResponse>> GetUserPetsAsync(int userId, CancellationToken cancellationToken = default);
    Task<PetResponse> GetPetByIdAsync(int petId, CancellationToken cancellationToken = default);
    Task AddPetToCurrentUserAsync(CreatePetDto dto, CancellationToken cancellationToken = default);
    Task UpdatePetAsync(int petId, CreatePetDto dto, CancellationToken cancellationToken = default);
    Task RemovePetAsync(int petId, CancellationToken cancellationToken = default);
}
