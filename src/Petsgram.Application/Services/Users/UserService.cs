using Petsgram.Application.Interfaces.Users;
using AutoMapper;
using Petsgram.Application.Interfaces.UnitOfWork;

namespace Petsgram.Application.Services.Users;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

}
