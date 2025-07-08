using Petsgram.Application.DTOs.Pets;
using Petsgram.Application.DTOs.Users;
using AutoMapper;
using Petsgram.Domain.Entities;

namespace Petsgram.Application.Mapper;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<AddUserDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Pets, opt => opt.Ignore());

        CreateMap<User, UserResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.Pets, opt =>
                opt.MapFrom(src => src.Pets.Select(p => new PetResponse
                {
                    PetName = p.PetName,
                    PetType = p.PetType.Name,
                    PublicUrls = p.Photos.Select(ph => ph.PublicUrl).ToList()
                }).ToList()));
    }
}
