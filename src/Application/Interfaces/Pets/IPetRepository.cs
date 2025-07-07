using Domain.Entities;

namespace Application.Interfaces.Pets;

public interface IPetRepository
{
    public Task<ICollection<Pet>> GetUserPetsByIdAsync(int userId);
    public Task<Pet> AddPetToUserAsync(Pet pet);
    public Task RemoveUserPetAsync(int petId);

    //update method not written
}
