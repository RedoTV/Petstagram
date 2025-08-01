using Petsgram.Application.DTOs.Users;
using Petsgram.Application.Interfaces.Users;
using AutoMapper;
using Petsgram.Domain.Entities;
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

    public async Task<IEnumerable<UserResponse>> GetAllAsync(int count, int skip)
    {
        var users = await _unitOfWork.Users.GetAllAsync(count, skip);
        return users.Select(u => _mapper.Map<UserResponse>(u));
    }

    public async Task<UserResponse> GetByIdAsync(int id)
    {
        var user = await _unitOfWork.Users.FindAsync(id);
        if (user == null)
            throw new ArgumentException($"User with id:{id} not found");

        return _mapper.Map<UserResponse>(user);
    }

    public async Task AddUserAsync(CreateUserDto userDto)
    {
        var user = _mapper.Map<User>(userDto);

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.CompleteAsync();
    }

    public async Task UpdateUserAsync(int id, CreateUserDto userDto)
    {
        var user = await _unitOfWork.Users.FindAsync(id);
        if (user == null)
            throw new ArgumentException($"User with id:{id} not found");

        user.UserName = userDto.UserName;

        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.CompleteAsync();
    }

    public async Task RemoveUserAsync(int id)
    {
        await _unitOfWork.Users.RemoveAsync(id);
        await _unitOfWork.CompleteAsync();
    }
}
