using Petsgram.Application.DTOs.Users;
using Petsgram.Application.Interfaces.Users;
using Petsgram.Application.Interfaces.Auth;
using Petsgram.Application.Generators;
using AutoMapper;
using Petsgram.Domain.Entities;
using Petsgram.Application.Interfaces.UnitOfWork;
using Petsgram.Domain.Enums;

namespace Petsgram.Application.Services.Users;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IRefreshTokenService _refreshTokenService;

    public UserService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IPasswordHasher passwordHasher,
        ITokenGenerator tokenGenerator,
        IRefreshTokenService refreshTokenService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
        _refreshTokenService = refreshTokenService;
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

    public async Task<AuthResponseDto> RegisterAsync(CreateUserDto userDto)
    {
        if (await _unitOfWork.Users.UserNameExistsAsync(userDto.UserName))
            throw new InvalidOperationException($"User with username '{userDto.UserName}' already exists");

        var hashedPassword = _passwordHasher.HashPassword(userDto.Password);

        var user = new User
        {
            UserName = userDto.UserName,
            HashedPassword = hashedPassword,
            Role = AuthRoles.PetOwner
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.CompleteAsync();

        var token = _tokenGenerator.GenerateToken(user);
        var userResponse = _mapper.Map<UserResponse>(user);

        await _refreshTokenService.StoreRefreshToken(user.Id, token.RefreshToken);

        return new AuthResponseDto
        {
            AccessToken = token.AccessToken,
            RefreshToken = token.RefreshToken,
            CreatedAt = token.CreatedAt,
            ExpiresAt = token.ExpiresAt,
            User = userResponse
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _unitOfWork.Users.GetByUserNameAsync(loginDto.UserName);
        if (user == null)
            throw new InvalidOperationException("Invalid username or password");

        if (!_passwordHasher.VerifyPassword(loginDto.Password, user.HashedPassword))
            throw new InvalidOperationException("Invalid username or password");

        var token = _tokenGenerator.GenerateToken(user);
        var userResponse = _mapper.Map<UserResponse>(user);

        await _refreshTokenService.StoreRefreshToken(user.Id, token.RefreshToken);

        return new AuthResponseDto
        {
            AccessToken = token.AccessToken,
            RefreshToken = token.RefreshToken,
            CreatedAt = token.CreatedAt,
            ExpiresAt = token.ExpiresAt,
            User = userResponse
        };
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var newToken = await _refreshTokenService.RefreshTokenAsync(refreshToken);
        var user = await _refreshTokenService.GetUserFromRefreshTokenAsync(newToken.RefreshToken);

        if (user == null)
            throw new InvalidOperationException("User not found");

        var userResponse = _mapper.Map<UserResponse>(user);

        return new AuthResponseDto
        {
            AccessToken = newToken.AccessToken,
            RefreshToken = newToken.RefreshToken,
            CreatedAt = newToken.CreatedAt,
            ExpiresAt = newToken.ExpiresAt,
            User = userResponse
        };
    }

    public async Task RemoveUserAsync(int id)
    {
        await _unitOfWork.Users.RemoveAsync(id);
        await _unitOfWork.CompleteAsync();
    }
}
