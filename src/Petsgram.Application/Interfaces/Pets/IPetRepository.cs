using Petsgram.Application.Interfaces.Shared;
using Petsgram.Domain.Entities;

namespace Petsgram.Application.Interfaces.Pets;

public interface IPetRepository : IGenericRepository<Pet>
{
    Task<List<Pet>> GetAllAsync(int userId, CancellationToken cancellationToken = default);
}
