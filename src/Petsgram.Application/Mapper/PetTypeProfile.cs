using AutoMapper;
using Petsgram.Application.DTOs.PetTypes;
using Petsgram.Domain.Entities;

namespace Petsgram.Application.Mapper;

public class PetTypeProfile : Profile
{
    public PetTypeProfile()
    {
        CreateMap<PetType, PetTypeResponse>();
    }
}