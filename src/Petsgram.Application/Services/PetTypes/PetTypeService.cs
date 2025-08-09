using Petsgram.Application.DTOs.PetTypes;
using Petsgram.Application.Interfaces.PetTypes;
using Petsgram.Application.Interfaces.UnitOfWork;
using AutoMapper;
using Petsgram.Domain.Entities;

namespace Petsgram.Application.Services.PetTypes;

public class PetTypeService : IPetTypeService
{
    private readonly IPetTypeRepository _petTypeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PetTypeService(IPetTypeRepository petTypeRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _petTypeRepository = petTypeRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<PetTypeResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var types = await _petTypeRepository.GetAllAsync(cancellationToken);
        return types.Select(t => _mapper.Map<PetTypeResponse>(t)).ToList();
    }

    public async Task<PetTypeResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var type = await _petTypeRepository.FindAsync(id, cancellationToken);
        if (type == null)
            throw new ArgumentException($"PetType with id:{id} not found");

        return _mapper.Map<PetTypeResponse>(type);
    }

    public async Task AddTypeAsync(string name, CancellationToken cancellationToken = default)
    {
        var type = new PetType { Name = name };
        await _petTypeRepository.AddAsync(type, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveTypeAsync(int id, CancellationToken cancellationToken = default)
    {
        await _petTypeRepository.RemoveAsync(id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateTypeAsync(int id, string name, CancellationToken cancellationToken = default)
    {
        var type = await _petTypeRepository.FindAsync(id, cancellationToken);
        if (type == null)
            throw new ArgumentException($"PetType with id:{id} not found");

        type.Name = name;
        await _petTypeRepository.UpdateAsync(type, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
