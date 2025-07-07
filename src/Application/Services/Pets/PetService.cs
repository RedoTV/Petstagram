using Application.DTOs.Pets;
using Application.Interfaces.Pets;
using AutoMapper;
using Domain.Entities;

namespace Application.Services.Pets;

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
        Pet pet = _mapper.Map<AddPetToUserDto, Pet>(petDto);
        pet.UserId = userId;
        await _petRepository.AddPetToUserAsync(pet);
    }

    public async Task<ICollection<PetResponse>> GetUserPets(int userId)
    {
        ICollection<Pet> pets = await _petRepository.GetUserPetsByIdAsync(userId);
        return pets.Select(p => _mapper.Map<Pet, PetResponse>(p)).ToList();
    }

    public async Task RemoveUserPet(int petId)
    {
        await _petRepository.RemoveUserPetAsync(petId);
    }
}