using Petsgram.Application.DTOs.Users;
using AutoMapper;
using Petsgram.Domain.Entities;

namespace Petsgram.Application.Mapper;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserResponse>();
        
        CreateMap<CreateUserRequest, User>();
        
        CreateMap<RefreshToken, RefreshTokenResponse>();
    }
}
