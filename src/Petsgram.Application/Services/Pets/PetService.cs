using Petsgram.Application.DTOs.Pets;
using Petsgram.Application.Interfaces.Pets;
using Petsgram.Application.Interfaces.UnitOfWork;
using Petsgram.Application.Interfaces.Auth;
using Petsgram.Application.Interfaces.PetTypes;
using AutoMapper;
using Petsgram.Domain.Entities;

namespace Petsgram.Application.Services.Pets;

public class PetService : IPetService
{
    private readonly IPetRepository _petRepository;
    private readonly IPetTypeRepository _petTypeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public PetService(
        IPetRepository petRepository,
        IPetTypeRepository petTypeRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _petRepository = petRepository;
        _petTypeRepository = petTypeRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<List<PetResponse>> GetCurrentUserPetsAsync(CancellationToken cancellationToken = default)
    {
        var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
        if (currentUser == null)
            throw new UnauthorizedAccessException("User not authenticated");

        var pets = await _petRepository.GetAllAsync(currentUser.Id, cancellationToken);
        return pets.Select(p => _mapper.Map<PetResponse>(p)).ToList();
    }

    public async Task<List<PetResponse>> GetUserPetsAsync(int userId, CancellationToken cancellationToken = default)
    {
        var pets = await _petRepository.GetAllAsync(userId, cancellationToken);
        return pets.Select(p => _mapper.Map<PetResponse>(p)).ToList();
    }

    public async Task<PetResponse> GetPetByIdAsync(int petId, CancellationToken cancellationToken = default)
    {
        var pet = await _petRepository.FindAsync(petId, cancellationToken);
        if (pet == null)
            throw new ArgumentException($"Pet with id:{petId} not found");

        return _mapper.Map<PetResponse>(pet);
    }

    public async Task AddPetToCurrentUserAsync(CreatePetDto dto, CancellationToken cancellationToken = default)
    {
        var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
        if (currentUser == null)
            throw new UnauthorizedAccessException("User not authenticated");

        var petType = await _petTypeRepository.GetByNameAsync(dto.PetType, cancellationToken);
        if (petType == null)
        {
            throw new ArgumentException($"Pet type '{dto.PetType}' not found.");
        }

        var pet = new Pet
        {
            PetName = dto.PetName,
            PetType = petType,
            UserId = currentUser.Id
        };

        await _petRepository.AddAsync(pet, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdatePetAsync(int petId, CreatePetDto dto, CancellationToken cancellationToken = default)
    {
        var pet = await _petRepository.FindAsync(petId, cancellationToken);
        if (pet == null)
            throw new ArgumentException($"Pet with id:{petId} not found");

        var petType = await _petTypeRepository.GetByNameAsync(dto.PetType, cancellationToken);
        if (petType == null)
        {
            throw new ArgumentException($"Pet type '{dto.PetType}' not found.");
        }

        pet.PetName = dto.PetName;
        pet.PetTypeId = petType.Id;

        await _petRepository.UpdateAsync(pet, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RemovePetAsync(int petId, CancellationToken cancellationToken = default)
    {
        await _petRepository.RemoveAsync(petId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
