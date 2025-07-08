using Petsgram.Application.DTOs.Pets;
using Petsgram.Application.Interfaces.Pets;
using AutoMapper;
using Petsgram.Domain.Entities;

namespace Petsgram.Application.Services.Pets;

public class PetService : IPetService
{
    private readonly IPetRepository _petRepository;
    private readonly IMapper _mapper;

    public PetService(IPetRepository petRepository, IMapper mapper)
    {
        _petRepository = petRepository;
        _mapper = mapper;
    }

    public async Task AddPetToUser(int userId, AddPetToUserDto petDto)
    {
        var pet = _mapper.Map<AddPetToUserDto, Pet>(petDto);
        pet.UserId = userId;
        await _petRepository.AddAsync(pet);
    }

    public async Task<ICollection<PetResponse>> GetUserPets(int userId)
    {
        var pets = await _petRepository.GetAsync(userId);
        return pets.Select(p => _mapper.Map<Pet, PetResponse>(p)).ToList();
    }

    public async Task RemoveUserPet(int petId)
    {
        await _petRepository.RemoveAsync(petId);
    }
}