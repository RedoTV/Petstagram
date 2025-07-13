using System.Threading.Tasks;
using Petsgram.Application.Interfaces.Pets;
using Petsgram.Application.Interfaces.UnitOfWork;
using Petsgram.Application.Interfaces.Users;
using Petsgram.Infrastructure.DbContexts;
using Petsgram.Infrastructure.Repositories;

namespace Petsgram.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly PetsgramDbContext _dbContext;

    private IPetRepository? _pets;
    private IUserRepository? _users;

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

    public async Task<int> CompleteAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContext.DisposeAsync();
    }
}
