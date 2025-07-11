using Petsgram.Application.DTOs.Pets;

namespace Petsgram.Application.Interfaces.Pets;

public interface IPetService
{
    Task<ICollection<PetResponse>> GetUserPetsAsync(int userId);
    Task AddPetToUserAsync(int userId, AddPetToUserAsyncDto pet);
    Task RemoveUserPetAsync(int petId);
}
