using Petsgram.Application.Interfaces.Shared;
using Petsgram.Domain.Entities;

namespace Petsgram.Application.Interfaces.PetPhotos;

public interface IPetPhotoRepository : IGenericRepository<PetPhoto>
{
    Task<IEnumerable<PetPhoto>> GetAllAsync(int petId);
}
