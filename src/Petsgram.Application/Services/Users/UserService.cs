using Petsgram.Application.DTOs.Users;
using Petsgram.Application.Interfaces.Users;
using AutoMapper;
using Petsgram.Domain.Entities;

namespace Petsgram.Application.Services.Users;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<ICollection<UserResponse>> GetUsersWithPetsAsync(int count, int skip)
    {
        var users = await _userRepository.GetUsersAsync(count, skip);
        return users.Select(u => _mapper.Map<User, UserResponse>(u)).ToList();
    }

    public async Task<UserResponse> GetUserByIdAsync(int userId)
    {
        var user = await _userRepository.GetUserByIdAsyncAsync(userId);
        return _mapper.Map<User, UserResponse>(user);
    }
}
