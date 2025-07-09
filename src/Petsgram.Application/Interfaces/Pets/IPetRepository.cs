using Petsgram.Domain.Entities;

namespace Petsgram.Application.Interfaces.Pets;

public interface IPetRepository
{
    Task<ICollection<Pet>> GetAsync(int userId);
    Task<Pet> AddAsync(Pet pet);
    Task RemoveAsync(int petId);

    //update method not written
}
