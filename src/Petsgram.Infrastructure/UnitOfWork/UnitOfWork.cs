using System.Threading.Tasks;
using Petsgram.Application.Interfaces.Pets;
using Petsgram.Application.Interfaces.UnitOfWork;
using Petsgram.Application.Interfaces.Users;
using Petsgram.Application.Interfaces.PetPhotos;
using Petsgram.Application.Interfaces.PetTypes;
using Petsgram.Infrastructure.DbContexts;
using Petsgram.Infrastructure.Repositories;

namespace Petsgram.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly PetsgramDbContext _dbContext;

    private IPetRepository? _pets;
    private IUserRepository? _users;
    private IPetPhotoRepository? _petPhotos;
    private IPetTypeRepository? _petTypes;

    public UnitOfWork(PetsgramDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IPetRepository Pets
    {
        get
        {
            if (_pets == null)
                _pets = new PetRepository(_dbContext);

            return _pets;
        }
    }

    public IUserRepository Users
    {
        get
        {
            if (_users == null)
                _users = new UserRepository(_dbContext);

            return _users;
        }
    }

    public IPetPhotoRepository PetPhotos
    {
        get
        {
            if (_petPhotos == null)
                _petPhotos = new PetPhotoRepository(_dbContext);

            return _petPhotos;
        }
    }

    public IPetTypeRepository PetTypes
    {
        get
        {
            if (_petTypes == null)
                _petTypes = new PetTypeRepository(_dbContext);

            return _petTypes;
        }
    }

    public async Task<int> CompleteAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContext.DisposeAsync();
    }
}