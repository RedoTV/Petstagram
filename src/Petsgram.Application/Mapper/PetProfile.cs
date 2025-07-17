using Petsgram.Application.DTOs.Pets;
using AutoMapper;
using Petsgram.Domain.Entities;

namespace Petsgram.Application.Mapper;

public class PetProfile : Profile
{
    public PetProfile()
    {
        CreateMap<CreatePetDto, Pet>();

        CreateMap<Pet, PetResponse>()
            .ForMember(dest => dest.PetType, opt => opt.MapFrom(src => src.PetType.Name))
            .ForMember(dest => dest.PublicUrls, opt =>
                opt.MapFrom(src => src.Photos.Select(p => p.PublicUrl).ToList()));
    }
}
