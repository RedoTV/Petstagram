using Petsgram.Application.DTOs.Pets;

namespace Petsgram.Application.Interfaces.Pets;

public interface IPetService
{
    Task<IEnumerable<PetResponse>> GetCurrentUserPetsAsync();
    Task<IEnumerable<PetResponse>> GetUserPetsAsync(int userId);
    Task<PetResponse> GetPetByIdAsync(int petId);
    Task AddPetToCurrentUserAsync(CreatePetDto pet);
    Task UpdatePetAsync(int petId, CreatePetDto pet);
    Task RemovePetAsync(int petId);
}
