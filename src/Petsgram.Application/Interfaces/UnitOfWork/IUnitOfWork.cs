using Petsgram.Application.Interfaces.PetPhotos;
using Petsgram.Application.Interfaces.Pets;
using Petsgram.Application.Interfaces.PetTypes;
using Petsgram.Application.Interfaces.Users;

namespace Petsgram.Application.Interfaces.UnitOfWork;

public interface IUnitOfWork
{
    IPetRepository Pets { get; }
    IUserRepository Users { get; }
    IPetTypeRepository PetTypes { get; }
    IPetPhotoRepository PetPhotos { get; }

    Task<int> CompleteAsync();
    Task DisposeAsync();
}
