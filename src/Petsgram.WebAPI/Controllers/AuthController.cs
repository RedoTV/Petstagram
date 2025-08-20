using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Petsgram.Application.DTOs.Users;
using Petsgram.Application.Interfaces.Users;
using Petsgram.Application.Interfaces.Auth;
using System.Security.Claims;

namespace Petsgram.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IRefreshTokenService _refreshTokenService;

    public AuthController(IUserService userService, IRefreshTokenService refreshTokenService)
    {
        _userService = userService;
        _refreshTokenService = refreshTokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _userService.RegisterAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _userService.LoginAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _userService.RefreshTokenAsync(request.AccessToken, request.RefreshToken, cancellationToken);
        return Ok(result);
    }

    [HttpPost("revoke")]
    [Authorize]
    public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest request, CancellationToken cancellationToken = default)
    {
        var revoked = await _refreshTokenService.RevokeTokenAsync(request.RefreshToken, cancellationToken);
        return Ok(revoked);
    }

    [HttpPost("revoke-all")]
    [Authorize]
    public async Task<IActionResult> RevokeAllTokens(CancellationToken cancellationToken = default)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            throw new UnauthorizedAccessException("Invalid token");

        var revoked = await _refreshTokenService.RevokeAllUserTokensAsync(userId, cancellationToken);
        return Ok(revoked);
    }
}