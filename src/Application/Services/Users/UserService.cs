using Application.DTOs.Users;
using Application.Interfaces.Users;
using AutoMapper;
using Domain.Entities;

namespace Application.Services.Users;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<ICollection<UserResponse>> GetUsersWithPets(int count, int skip)
    {
        ICollection<User> users = await _userRepository.GetUsersAsync(count, skip);
        return users.Select(u => _mapper.Map<User, UserResponse>(u)).ToList();
    }

    public async Task<UserResponse> GetUserById(int userId)
    {
        User user = await _userRepository.GetUserByIdAsync(userId);
        return _mapper.Map<User, UserResponse>(user);
    }
}
