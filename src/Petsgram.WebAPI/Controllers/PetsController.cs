using Petsgram.Application.DTOs.Pets;
using Petsgram.Application.Interfaces.Pets;
using Microsoft.AspNetCore.Mvc;

namespace Petsgram.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PetsController : ControllerBase
{
    private readonly ILogger<PetsController> _logger;
    private readonly IPetService _petService;

    public PetsController(ILogger<PetsController> logger, IPetService petService)
    {
        _logger = logger;
        _petService = petService;
    }

    [HttpGet("getPets/{userId}")]
    public async Task<IActionResult> GetPets(int userId)
    {
        try
        {
            var pets = await _petService.GetUserPets(userId);
            return Ok(new { pets });
        }
        catch (Exception exc)
        {
            _logger.LogInformation($"Pets not found for user with id:{userId}, \nerror:{exc}");
            return BadRequest(new { message = "Pets not found" });
        }
    }

    [HttpPost("addPet/{userId}")]
    public async Task<IActionResult> AddPet(int userId, AddPetToUserDto pet)
    {
        try
        {
            await _petService.AddPetToUser(userId, pet);
            return Ok(new { message = "Pet added" });
        }
        catch (Exception exc)
        {
            _logger.LogError($"Pet not added for user with id:{userId}, \nerror:{exc}");
            return BadRequest(new { message = "Pet not added" });
        }
    }

    [HttpDelete("removePet/{petId}")]
    public async Task<IActionResult> RemovePet(int petId)
    {
        try
        {
            await _petService.RemoveUserPet(petId);
            return Ok(new { message = "Pet with id removed" });
        }
        catch (Exception exc)
        {
            _logger.LogError($"Pet not removed \nerror:{exc}");
            return BadRequest(new { message = "Pet not removed" });
        }
    }
}
