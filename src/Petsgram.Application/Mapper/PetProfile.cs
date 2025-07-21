using Petsgram.Application.DTOs.Pets;
using AutoMapper;
using Petsgram.Domain.Entities;

namespace Petsgram.Application.Mapper;

public class PetProfile : Profile
{
    public PetProfile()
    {
        CreateMap<CreatePetDto, Pet>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.PetTypeId, opt => opt.Ignore())
            .ForMember(dest => dest.PetType, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Photos, opt => opt.Ignore()
        );

        CreateMap<Pet, PetResponse>()
            .ForMember(dest => dest.PetType, opt => opt.MapFrom(src => src.PetType.Name))
            .ForMember(dest => dest.PublicUrls, opt =>
                opt.MapFrom(src => src.Photos.Select(p => p.PublicUrl).ToList()
            )
        );
    }
}
