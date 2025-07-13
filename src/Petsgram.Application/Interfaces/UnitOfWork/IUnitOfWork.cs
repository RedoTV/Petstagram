namespace Petsgram.Application.Interfaces.UnitOfWork;

public interface IUnitOfWork
{
    Task<int> CompleteAsync();
    Task DisposeAsync();
}
