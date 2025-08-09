using Petsgram.Application.DTOs.PetTypes;

namespace Petsgram.Application.Interfaces.PetTypes;

public interface IPetTypeService
{
    Task<List<PetTypeResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PetTypeResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddTypeAsync(string name, CancellationToken cancellationToken = default);
    Task RemoveTypeAsync(int id, CancellationToken cancellationToken = default);
    Task UpdateTypeAsync(int id, string name, CancellationToken cancellationToken = default);
}