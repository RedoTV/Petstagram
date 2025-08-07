using Petsgram.Application.DTOs.Users;
using Petsgram.Application.Interfaces.Users;
using Petsgram.Application.Interfaces.Auth;
using Petsgram.Application.Interfaces.UnitOfWork;
using Petsgram.Application.Generators;
using AutoMapper;
using Petsgram.Domain.Entities;
using Petsgram.Domain.Enums;

namespace Petsgram.Application.Services.Users;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMapper _mapper;

    public UserService(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ITokenGenerator tokenGenerator,
        IRefreshTokenService refreshTokenService,
        IPasswordHasher passwordHasher,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _tokenGenerator = tokenGenerator;
        _refreshTokenService = refreshTokenService;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
    }

    public async Task<List<UserResponse>> GetAllAsync(int count, int skip, CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(count, skip, cancellationToken);
        return users.Select(u => _mapper.Map<UserResponse>(u)).ToList();
    }

    public async Task<UserResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindAsync(id, cancellationToken);
        if (user == null)
            throw new ArgumentException($"User with id:{id} not found");

        return _mapper.Map<UserResponse>(user);
    }

    public async Task<AuthResponseDto> RegisterAsync(CreateUserDto userDto, CancellationToken cancellationToken = default)
    {
        if (await _userRepository.UserNameExistsAsync(userDto.UserName, cancellationToken))
            throw new ArgumentException($"User with username:{userDto.UserName} already exists");

        var hashedPassword = _passwordHasher.HashPassword(userDto.Password);
        var user = new User
        {
            UserName = userDto.UserName,
            HashedPassword = hashedPassword,
            Role = AuthRoles.PetOwner
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var token = _tokenGenerator.GenerateToken(user);
        await _refreshTokenService.StoreRefreshToken(user.Id, token.RefreshToken, cancellationToken);

        return new AuthResponseDto
        {
            AccessToken = token.AccessToken,
            RefreshToken = token.RefreshToken,
            ExpiresAt = token.ExpiresAt,
            User = _mapper.Map<UserResponse>(user)
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByUserNameAsync(loginDto.UserName, cancellationToken);
        if (user == null || !_passwordHasher.VerifyPassword(loginDto.Password, user.HashedPassword))
        {
            throw new InvalidOperationException("Invalid username or password");
        }

        var token = _tokenGenerator.GenerateToken(user);

        await _refreshTokenService.StoreRefreshToken(user.Id, token.RefreshToken, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResponseDto
        {
            AccessToken = token.AccessToken,
            RefreshToken = token.RefreshToken,
            ExpiresAt = token.ExpiresAt,
            User = _mapper.Map<UserResponse>(user)
        };
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string accessToken, string refreshToken, CancellationToken cancellationToken = default)
    {
        var user = await _refreshTokenService.GetUserFromRefreshTokenAsync(refreshToken, cancellationToken);
        if (user == null)
            throw new ArgumentException("User not found");

        var newToken = await _refreshTokenService.RefreshTokenAsync(accessToken, refreshToken, cancellationToken);

        return new AuthResponseDto
        {
            AccessToken = newToken.AccessToken,
            RefreshToken = newToken.RefreshToken,
            ExpiresAt = newToken.ExpiresAt,
            User = _mapper.Map<UserResponse>(user)
        };
    }


    public async Task RemoveUserAsync(int id)
    {
        var user = await _userRepository.FindAsync(id);
        if (user == null)
            throw new KeyNotFoundException($"User with id {id} not found");

        await _refreshTokenService.RevokeAllUserTokensAsync(id);

        await _userRepository.RemoveAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }
}
