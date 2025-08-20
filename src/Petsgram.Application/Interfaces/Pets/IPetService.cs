using Petsgram.Application.DTOs.Pets;

namespace Petsgram.Application.Interfaces.Pets;

public interface IPetService
{
    Task<List<PetResponse>> GetCurrentUserPetsAsync(CancellationToken cancellationToken = default);
    Task<List<PetResponse>> GetUserPetsAsync(int userId, CancellationToken cancellationToken = default);
    Task<PetResponse> GetPetByIdAsync(int petId, CancellationToken cancellationToken = default);
    Task<PetResponse> AddPetToCurrentUserAsync(CreatePetRequest request, CancellationToken cancellationToken = default);
    Task<PetResponse> UpdatePetAsync(int petId, UpdatePetRequest request, CancellationToken cancellationToken = default);
    Task<PetResponse> RemovePetAsync(int petId, CancellationToken cancellationToken = default);
}