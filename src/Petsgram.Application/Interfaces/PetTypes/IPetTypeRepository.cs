using Petsgram.Application.Interfaces.Shared;
using Petsgram.Domain.Entities;

namespace Petsgram.Application.Interfaces.PetTypes;

public interface IPetTypeRepository : IGenericRepository<PetType>
{
    Task<IEnumerable<PetType>> GetAllAsync();
}
