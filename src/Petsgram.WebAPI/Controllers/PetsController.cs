using Petsgram.Application.DTOs.Pets;
using Petsgram.Application.Interfaces.Pets;
using Microsoft.AspNetCore.Mvc;

namespace Petsgram.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PetsController : ControllerBase
{
    private readonly ILogger<PetsController> _logger;
    private readonly IPetService _petService;

    public PetsController(ILogger<PetsController> logger, IPetService petService)
    {
        _logger = logger;
        _petService = petService;
    }

    [HttpGet("by-user/{userId}")]
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
        catch (Exception exc)
        {
            _logger.LogError($"Pet not found with id:{petId}, error:{exc}");
            return BadRequest(new { message = "Pet not found" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(int userId, [FromBody] CreatePetDto dto)
    {
        try
        {
            await _petService.AddPetToUserAsync(userId, dto);
            _logger.LogInformation($"Pet created for user {userId}");
            return Ok();
        }
        catch (Exception exc)
        {
            _logger.LogError($"Pet not created for user {userId}, error:{exc}");
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
            return Ok();
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
            await _petService.RemoveUserPetAsync(petId);
            _logger.LogInformation($"Pet deleted: {petId}");
            return Ok();
        }
        catch (Exception exc)
        {
            _logger.LogError($"Pet not deleted: {petId}, error:{exc}");
            return BadRequest(new { message = "Pet not deleted" });
        }
    }
}
