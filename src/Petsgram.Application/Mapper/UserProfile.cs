using Petsgram.Application.DTOs.Pets;
using Petsgram.Application.DTOs.Users;
using AutoMapper;
using Petsgram.Domain.Entities;

namespace Petsgram.Application.Mapper;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<CreateUserDto, User>();
        CreateMap<User, UserResponse>();
    }
}
