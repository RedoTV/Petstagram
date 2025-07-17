using Petsgram.Application.Interfaces.Shared;
using Petsgram.Domain.Entities;

namespace Petsgram.Application.Interfaces.Pets;

public interface IPetRepository : IGenericRepository<Pet>
{
    Task<IEnumerable<Pet>> GetAllAsync(int userId);
}
