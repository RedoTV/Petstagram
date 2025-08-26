using Petsgram.Application.DTOs.Pets;

namespace Petsgram.Application.Interfaces.Pets;

public interface IPetService
{
    Task<List<PetResponse>> GetCurrentUserPetsAsync(string? targetCurrency = null, CancellationToken cancellationToken = default);
    Task<List<PetResponse>> GetUserPetsAsync(int userId, string? targetCurrency = null, CancellationToken cancellationToken = default);
    Task<PetResponse> GetPetByIdAsync(int petId, string? targetCurrency = null, CancellationToken cancellationToken = default);
    Task<PetResponse> AddPetToCurrentUserAsync(CreatePetRequest request, CancellationToken cancellationToken = default);
    Task<PetResponse> UpdatePetAsync(int petId, UpdatePetRequest request, CancellationToken cancellationToken = default);
    Task<PetResponse> RemovePetAsync(int petId, CancellationToken cancellationToken = default);
}