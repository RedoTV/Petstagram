using Petsgram.Application.DTOs.PetTypes;
using Petsgram.Application.Interfaces.PetTypes;
using Petsgram.Application.Interfaces.UnitOfWork;
using AutoMapper;
using Petsgram.Domain.Entities;

namespace Petsgram.Application.Services.PetTypes;

public class PetTypeService : IPetTypeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PetTypeService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PetTypeResponse>> GetAllAsync()
    {
        var types = await _unitOfWork.PetTypes.GetAllAsync();
        return types.Select(t => _mapper.Map<PetTypeResponse>(t)).ToList();
    }

    public async Task<PetTypeResponse> GetByIdAsync(int id)
    {
        var type = await _unitOfWork.PetTypes.FindAsync(id);
        if (type == null)
            throw new ArgumentException($"PetType with id:{id} not found");

        return _mapper.Map<PetTypeResponse>(type);
    }

    public async Task AddTypeAsync(string name)
    {
        var type = new PetType { Name = name };

        await _unitOfWork.PetTypes.AddAsync(type);
        await _unitOfWork.CompleteAsync();
    }

    public async Task RemoveTypeAsync(int id)
    {
        await _unitOfWork.PetTypes.RemoveAsync(id);
        await _unitOfWork.CompleteAsync();
    }

    public async Task UpdateTypeAsync(int id, string name)
    {
        var type = await _unitOfWork.PetTypes.FindAsync(id);
        if (type == null)
            throw new ArgumentException($"PetType with id:{id} not found");

        type.Name = name;

        await _unitOfWork.PetTypes.UpdateAsync(type);
        await _unitOfWork.CompleteAsync();
    }
}