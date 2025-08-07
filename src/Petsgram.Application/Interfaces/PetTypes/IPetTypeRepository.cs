using Petsgram.Application.Interfaces.Shared;
using Petsgram.Domain.Entities;

namespace Petsgram.Application.Interfaces.PetTypes;

public interface IPetTypeRepository : IGenericRepository<PetType>
{
    Task<List<PetType>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PetType?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
