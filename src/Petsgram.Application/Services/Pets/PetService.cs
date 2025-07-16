using Petsgram.Application.DTOs.Pets;
using Petsgram.Application.Interfaces.Pets;
using AutoMapper;
using Petsgram.Domain.Entities;
using Petsgram.Application.Interfaces.UnitOfWork;

namespace Petsgram.Application.Services.Pets;

public class PetService : IPetService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PetService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PetResponse>> GetUserPetsAsync(int userId)
    {
        var pets = await _unitOfWork.Pets.GetAllAsync(userId);
        return pets.Select(p => _mapper.Map<PetResponse>(p));
    }

    public async Task<PetResponse> GetPetByIdAsync(int petId)
    {
        var pet = await _unitOfWork.Pets.FindAsync(petId);
        if (pet == null)
            throw new ArgumentException($"Pet with id:{petId} not found");

        return _mapper.Map<PetResponse>(pet);
    }

    public async Task AddPetToUserAsync(int userId, CreatePetDto petDto)
    {
        var petType = (await _unitOfWork.PetTypes.GetAllAsync())
            .FirstOrDefault(pt => pt.Name == petDto.PetType);
        if (petType == null)
        {
            petType = new PetType { Name = petDto.PetType };
            await _unitOfWork.PetTypes.AddAsync(petType);
        }

        var pet = _mapper.Map<Pet>(petDto);
        pet.UserId = userId;
        pet.PetTypeId = petType.Id;

        await _unitOfWork.Pets.AddAsync(pet);
        await _unitOfWork.CompleteAsync();
    }

    public async Task UpdatePetAsync(int petId, CreatePetDto petDto)
    {
        var pet = await _unitOfWork.Pets.FindAsync(petId);
        if (pet == null)
            throw new ArgumentException($"Pet with id:{petId} not found");

        var petType = (await _unitOfWork.PetTypes.GetAllAsync())
            .FirstOrDefault(pt => pt.Name == petDto.PetType);
        if (petType == null)
        {
            petType = new PetType { Name = petDto.PetType };
            await _unitOfWork.PetTypes.AddAsync(petType);
        }

        pet.PetName = petDto.PetName;
        pet.PetTypeId = petType.Id;

        await _unitOfWork.Pets.UpdateAsync(pet);
        await _unitOfWork.CompleteAsync();
    }

    public async Task RemoveUserPetAsync(int petId)
    {
        await _unitOfWork.Pets.RemoveAsync(petId);
        await _unitOfWork.CompleteAsync();
    }
}