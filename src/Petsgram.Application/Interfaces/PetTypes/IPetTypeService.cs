using Petsgram.Application.DTOs.PetTypes;

namespace Petsgram.Application.Interfaces.PetTypes;

public interface IPetTypeService
{
    Task<List<PetTypeResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PetTypeResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PetTypeResponse> AddTypeAsync(PetTypeCreateRequest request, CancellationToken cancellationToken = default);
    Task<PetTypeResponse> UpdateTypeAsync(int id, PetTypeUpdateRequest request, CancellationToken cancellationToken = default);
    Task<PetTypeResponse> RemoveTypeAsync(int id, CancellationToken cancellationToken = default);
}