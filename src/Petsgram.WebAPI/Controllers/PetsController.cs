using Petsgram.Application.DTOs.Pets;
using Petsgram.Application.Interfaces.Pets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Petsgram.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PetsController : ControllerBase
{
    private readonly ILogger<PetsController> _logger;
    private readonly IPetService _petService;

    public PetsController(ILogger<PetsController> logger, IPetService petService)
    {
        _logger = logger;
        _petService = petService;
    }

    [HttpGet("my-pets")]
    public async Task<IActionResult> GetCurrentUserPets()
    {
        try
        {
            var pets = await _petService.GetCurrentUserPetsAsync();
            _logger.LogInformation($"Returned {pets.Count()} pets for current user");
            return Ok(pets);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError($"Unauthorized access: {ex.Message}");
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception exc)
        {
            _logger.LogError($"Error getting current user pets: {exc}");
            return BadRequest(new { message = "Error getting pets" });
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetAllByUser(int userId)
    {
        try
        {
            var pets = await _petService.GetUserPetsAsync(userId);
            _logger.LogInformation($"Returned {pets.Count()} pets for user {userId}");
            return Ok(pets);
        }
        catch (Exception exc)
        {
            _logger.LogError($"Error getting pets for user {userId}: {exc}");
            return BadRequest(new { message = "Error getting pets" });
        }
    }

    [HttpGet("{petId}")]
    public async Task<IActionResult> GetById(int petId)
    {
        try
        {
            var pet = await _petService.GetPetByIdAsync(petId);
            _logger.LogInformation($"Returned pet with id:{petId}");
            return Ok(pet);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError($"Pet not found with id:{petId}, error:{ex}");
            return NotFound(new { message = ex.Message });
        }
        catch (Exception exc)
        {
            _logger.LogError($"Error getting pet with id:{petId}, error:{exc}");
            return BadRequest(new { message = "Error getting pet" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePetDto dto)
    {
        try
        {
            await _petService.AddPetToCurrentUserAsync(dto);
            _logger.LogInformation("Pet created for current user");
            return Ok(new { message = "Pet created successfully" });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError($"Unauthorized access: {ex.Message}");
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception exc)
        {
            _logger.LogError($"Pet not created, error:{exc}");
            return BadRequest(new { message = "Pet not created" });
        }
    }

    [HttpPut("{petId}")]
    public async Task<IActionResult> Update(int petId, [FromBody] CreatePetDto dto)
    {
        try
        {
            await _petService.UpdatePetAsync(petId, dto);
            _logger.LogInformation($"Pet updated: {petId}");
            return Ok(new { message = "Pet updated successfully" });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError($"Unauthorized access: {ex.Message}");
            return Unauthorized(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogError($"Pet not found with id:{petId}, error:{ex}");
            return NotFound(new { message = ex.Message });
        }
        catch (Exception exc)
        {
            _logger.LogError($"Pet not updated: {petId}, error:{exc}");
            return BadRequest(new { message = "Pet not updated" });
        }
    }

    [HttpDelete("{petId}")]
    public async Task<IActionResult> Delete(int petId)
    {
        try
        {
            await _petService.RemovePetAsync(petId);
            _logger.LogInformation($"Pet deleted: {petId}");
            return Ok(new { message = "Pet deleted successfully" });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError($"Unauthorized access: {ex.Message}");
            return Unauthorized(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogError($"Pet not found with id:{petId}, error:{ex}");
            return NotFound(new { message = ex.Message });
        }
        catch (Exception exc)
        {
            _logger.LogError($"Pet not deleted: {petId}, error:{exc}");
            return BadRequest(new { message = "Pet not deleted" });
        }
    }
}
