using Petsgram.Application.DTOs.PetTypes;
using Petsgram.Application.Interfaces.PetTypes;
using Petsgram.Application.Interfaces.UnitOfWork;
using AutoMapper;
using Petsgram.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Petsgram.Domain.Exceptions.PetType;

namespace Petsgram.Application.Services.PetTypes;

public class PetTypeService : IPetTypeService
{
    private readonly IPetTypeRepository _petTypeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<PetTypeService> _logger;

    public PetTypeService(
        IPetTypeRepository petTypeRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<PetTypeService> logger)
    {
        _petTypeRepository = petTypeRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<PetTypeResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var types = await _petTypeRepository.GetAllAsync(cancellationToken);
        _logger.LogInformation("Returned {Count} pet types", types.Count);
        return _mapper.Map<List<PetTypeResponse>>(types);
    }

    public async Task<PetTypeResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var type = await _petTypeRepository.FindAsync(id, cancellationToken);
        if (type == null)
        {
            _logger.LogWarning("Pet type with id {Id} not found", id);
            throw new PetTypeNotFoundException(id);
        }

        _logger.LogInformation("Returned pet type with id {Id}", id);
        return _mapper.Map<PetTypeResponse>(type);
    }

    public async Task<PetTypeResponse> AddTypeAsync(PetTypeCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new PetTypeValidationException("Pet type name cannot be empty");

        var existing = await _petTypeRepository.GetByNameAsync(request.Name, cancellationToken);
        if (existing != null)
            throw new PetTypeAlreadyExistsException(request.Name);

        var type = _mapper.Map<PetType>(request);
        await _petTypeRepository.AddAsync(type, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Pet type created: {Name}", request.Name);
        return _mapper.Map<PetTypeResponse>(type);
    }

    public async Task<PetTypeResponse> UpdateTypeAsync(int id, PetTypeUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new PetTypeValidationException("Pet type name cannot be empty");

        var type = await _petTypeRepository.FindAsync(id, cancellationToken);
        if (type == null)
            throw new PetTypeNotFoundException(id);

        var existing = await _petTypeRepository.GetByNameAsync(request.Name, cancellationToken);
        if (existing != null && existing.Id != id)
            throw new PetTypeAlreadyExistsException(request.Name);

        _mapper.Map(request, type);
        await _petTypeRepository.UpdateAsync(type, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
 
        _logger.LogInformation("Pet type updated: id={Id}, name={Name}", id, request.Name);
        return _mapper.Map<PetTypeResponse>(type);
    }

    public async Task<PetTypeResponse> RemoveTypeAsync(int id, CancellationToken cancellationToken = default)
    {
        var type = await _petTypeRepository.FindAsync(id, cancellationToken);
        if (type == null)
            throw new PetTypeNotFoundException(id);

        await _petTypeRepository.RemoveAsync(id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Pet type deleted: id={Id}", id);
        return _mapper.Map<PetTypeResponse>(type);
    }
}
