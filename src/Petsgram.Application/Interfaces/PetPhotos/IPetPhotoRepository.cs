using Petsgram.Application.Interfaces.Shared;
using Petsgram.Domain.Entities;

namespace Petsgram.Application.Interfaces.PetPhotos;

public interface IPetPhotoRepository : IGenericRepository<PetPhoto>
{
    Task<List<PetPhoto>> GetAllAsync(int petId, CancellationToken cancellationToken = default);
}
