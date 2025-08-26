using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Petsgram.Application.Interfaces.Users;

namespace Petsgram.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int count = 10, [FromQuery] int skip = 0, CancellationToken cancellationToken = default)
    {
        var users = await _userService.GetAllAsync(count, skip, cancellationToken);
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var user = await _userService.GetByIdAsync(id, cancellationToken);
        return Ok(user);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await _userService.RemoveUserAsync(id, cancellationToken);
        return Ok(deleted);
    }
}