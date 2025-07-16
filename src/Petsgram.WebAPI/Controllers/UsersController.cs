using Microsoft.AspNetCore.Mvc;
using Petsgram.Application.Interfaces.Users;
using Petsgram.Application.DTOs.Users;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Petsgram.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IUserService _userService;
    public UsersController(ILogger<UsersController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int count = 10, [FromQuery] int skip = 0)
    {
        try
        {
            var users = await _userService.GetAllAsync(count, skip);
            _logger.LogInformation($"Returned {users.Count()} users");
            return Ok(users);
        }
        catch (Exception exc)
        {
            _logger.LogError($"Error getting users: {exc}");
            return BadRequest(new { message = "Error getting users" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var user = await _userService.GetByIdAsync(id);
            _logger.LogInformation($"Returned user with id:{id}");
            return Ok(user);
        }
        catch (Exception exc)
        {
            _logger.LogError($"User not found with id:{id}, error:{exc}");
            return BadRequest(new { message = "User not found" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        try
        {
            await _userService.AddUserAsync(dto);
            _logger.LogInformation($"User created: {dto.UserName}");
            return Ok();
        }
        catch (Exception exc)
        {
            _logger.LogError($"User not created, error:{exc}");
            return BadRequest(new { message = "User not created" });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateUserDto dto)
    {
        try
        {
            await _userService.UpdateUserAsync(id, dto);
            _logger.LogInformation($"User updated: {id}");
            return Ok();
        }
        catch (Exception exc)
        {
            _logger.LogError($"User not updated: {id}, error:{exc}");
            return BadRequest(new { message = "User not updated" });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _userService.RemoveUserAsync(id);
            _logger.LogInformation($"User deleted: {id}");
            return Ok();
        }
        catch (Exception exc)
        {
            _logger.LogError($"User not deleted: {id}, error:{exc}");
            return BadRequest(new { message = "User not deleted" });
        }
    }
}