using Petsgram.Application.DTOs.Pets;

namespace Petsgram.Application.Interfaces.Pets;

public interface IPetService
{
    Task<IEnumerable<PetResponse>> GetUserPetsAsync(int userId);
    Task<PetResponse> GetPetByIdAsync(int petId);
    Task AddPetToUserAsync(int userId, CreatePetDto pet);
    Task UpdatePetAsync(int petId, CreatePetDto pet);
    Task RemoveUserPetAsync(int petId);
}
