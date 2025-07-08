using Petsgram.Application.DTOs.Pets;

namespace Petsgram.Application.Interfaces.Pets;

public interface IPetService
{
    Task<ICollection<PetResponse>> GetUserPets(int userId);
    Task AddPetToUser(int userId, AddPetToUserDto pet);
    Task RemoveUserPet(int petId);
}
