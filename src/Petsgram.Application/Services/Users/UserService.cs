using Petsgram.Application.DTOs.Users;
using Petsgram.Application.Interfaces.Users;
using Petsgram.Application.Interfaces.Auth;
using Petsgram.Application.Interfaces.UnitOfWork;
using Petsgram.Application.Generators;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Petsgram.Domain.Entities;
using Petsgram.Domain.Enums;
using Petsgram.Domain.Exceptions.Auth;
using Petsgram.Domain.Exceptions.User;

namespace Petsgram.Application.Services.Users;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ITokenGenerator tokenGenerator,
        IRefreshTokenService refreshTokenService,
        IPasswordHasher passwordHasher,
        IMapper mapper,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _tokenGenerator = tokenGenerator;
        _refreshTokenService = refreshTokenService;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<UserResponse>> GetAllAsync(int count, int skip, CancellationToken cancellationToken = default)
    {
        if (count <= 0) count = 10;
        if (skip < 0) skip = 0;

        var users = await _userRepository.GetAllAsync(count, skip, cancellationToken);
        _logger.LogInformation("Returned {Count} users with skip {Skip}", users.Count, skip);
        return _mapper.Map<List<UserResponse>>(users);
    }

    public async Task<UserResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindAsync(id, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("User with id {UserId} not found", id);
            throw new UserNotFoundException(id);
        }

        _logger.LogInformation("Returned user with id {UserId}", id);
        return _mapper.Map<UserResponse>(user);
    }

    public async Task<AuthResponse> RegisterAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.UserName))
            throw new UserValidationException("Username cannot be empty");

        if (string.IsNullOrWhiteSpace(request.Password))
            throw new UserValidationException("Password cannot be empty");

        if (await _userRepository.UserNameExistsAsync(request.UserName, cancellationToken))
        {
            _logger.LogWarning("Registration failed: username '{UserName}' already exists", request.UserName);
            throw new UserAlreadyExistsException(request.UserName);
        }

        var hashedPassword = _passwordHasher.HashPassword(request.Password);
        var user = new User
        {
            UserName = request.UserName,
            HashedPassword = hashedPassword,
            Role = AuthRoles.PetOwner
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var token = _tokenGenerator.GenerateToken(user);
        await _refreshTokenService.StoreRefreshToken(user.Id, token.RefreshToken, cancellationToken);

        _logger.LogInformation("User registered successfully: {UserName}", request.UserName);
        return new AuthResponse
        {
            AccessToken = token.AccessToken,
            RefreshToken = token.RefreshToken,
            ExpiresAt = token.ExpiresAt,
            User = _mapper.Map<UserResponse>(user)
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.UserName))
            throw new UserValidationException("Username cannot be empty");

        if (string.IsNullOrWhiteSpace(request.Password))
            throw new UserValidationException("Password cannot be empty");

        var user = await _userRepository.GetByUserNameAsync(request.UserName, cancellationToken);
        if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.HashedPassword))
        {
            _logger.LogWarning("Login failed for username: {UserName}", request.UserName);
            throw new AuthenticationException("Invalid username or password");
        }

        var token = _tokenGenerator.GenerateToken(user);
        await _refreshTokenService.StoreRefreshToken(user.Id, token.RefreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User logged in successfully: {UserName}", request.UserName);
        return new AuthResponse
        {
            AccessToken = token.AccessToken,
            RefreshToken = token.RefreshToken,
            ExpiresAt = token.ExpiresAt,
            User = _mapper.Map<UserResponse>(user)
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync(string accessToken, string refreshToken, CancellationToken cancellationToken = default)
    {
        var user = await _refreshTokenService.GetUserFromRefreshTokenAsync(refreshToken, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("Refresh token failed: user not found for token");
            throw new AuthenticationException("Invalid refresh token");
        }

        var newToken = await _refreshTokenService.RefreshTokenAsync(accessToken, refreshToken, cancellationToken);

        _logger.LogInformation("Token refreshed successfully for user {UserId}", user.Id);
        return new AuthResponse
        {
            AccessToken = newToken.AccessToken,
            RefreshToken = newToken.RefreshToken,
            ExpiresAt = newToken.ExpiresAt,
            User = _mapper.Map<UserResponse>(user)
        };
    }

    public async Task<UserResponse> RemoveUserAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindAsync(id, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("User with id {UserId} not found for deletion", id);
            throw new UserNotFoundException(id);
        }

        await _refreshTokenService.RevokeAllUserTokensAsync(id, cancellationToken);
        await _userRepository.RemoveAsync(id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User deleted: id={UserId}", id);
        return _mapper.Map<UserResponse>(user);
    }
}