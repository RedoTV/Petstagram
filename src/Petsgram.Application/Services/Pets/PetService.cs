using Petsgram.Application.DTOs.Pets;
using Petsgram.Application.Interfaces.Pets;
using AutoMapper;
using Petsgram.Domain.Entities;
using Petsgram.Application.Interfaces.UnitOfWork;

namespace Petsgram.Application.Services.Pets;

public class PetService : IPetService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PetService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

}