using AutoMapper;
using Petsgram.Application.DTOs.PetPhotos;
using Petsgram.Domain.Entities;

namespace Petsgram.Application.Mapper;

public class PetPhotoProfile : Profile
{
    public PetPhotoProfile()
    {
        CreateMap<PetPhoto, PetPhotoResponse>();
    }
}