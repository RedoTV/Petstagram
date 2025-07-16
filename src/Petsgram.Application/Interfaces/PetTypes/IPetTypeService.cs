using Petsgram.Application.DTOs.PetTypes;

namespace Petsgram.Application.Interfaces.PetTypes;

public interface IPetTypeService
{
    Task<IEnumerable<PetTypeResponse>> GetAllAsync();
    Task<PetTypeResponse> GetByIdAsync(int id);
    Task AddTypeAsync(string name);
    Task RemoveTypeAsync(int id);
    Task UpdateTypeAsync(int id, string name);
}