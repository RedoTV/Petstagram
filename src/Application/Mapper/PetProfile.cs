using Application.DTOs.Pets;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapper;

public class PetProfile : Profile
{
    public PetProfile()
    {
        CreateMap<AddPetToUserDto, Pet>()
            .ForMember(dest => dest.PetName, opt => opt.MapFrom(src => src.PetName))
            .ForMember(dest => dest.Photos, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .AfterMap((src, dest) => dest.PetType = new PetType { Name = src.PetType });

        CreateMap<Pet, PetResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.PetName, opt => opt.MapFrom(src => src.PetName))
            .ForMember(dest => dest.PetType, opt => opt.MapFrom(src => src.PetType.Name))
            .ForMember(dest => dest.PublicUrls, opt =>
                opt.MapFrom(src => src.Photos.Select(p => p.PublicUrl).ToList()));
    }
}
