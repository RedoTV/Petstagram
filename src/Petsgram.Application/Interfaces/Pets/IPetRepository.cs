using Petsgram.Domain.Entities;

namespace Petsgram.Application.Interfaces.Pets;

public interface IPetRepository
{
    public Task<ICollection<Pet>> GetAsync(int userId);
    public Task<Pet> AddAsync(Pet pet);
    public Task RemoveAsync(int petId);

    //update method not written
}
