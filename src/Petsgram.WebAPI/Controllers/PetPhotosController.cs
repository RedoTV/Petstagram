using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Petsgram.Application.Interfaces.PetPhotos;
using Petsgram.Application.DTOs.PetPhotos;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Petsgram.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PetPhotosController : ControllerBase
{
    private readonly ILogger<PetPhotosController> _logger;
    private readonly IPetPhotoService _petPhotoService;

    public PetPhotosController(ILogger<PetPhotosController> logger, IPetPhotoService petPhotoService)
    {
        _logger = logger;
        _petPhotoService = petPhotoService;
    }

    [HttpGet("by-pet/{petId}")]
    public async Task<IActionResult> GetAllByPet(int petId)
    {
        try
        {
            var photos = await _petPhotoService.GetAllByPetIdAsync(petId);
            _logger.LogInformation($"Returned {photos.Count()} photos for pet {petId}");

            return Ok(new { photos });
        }
        catch (Exception exc)
        {
            _logger.LogError($"Error getting photos for pet {petId}: {exc}");
            return BadRequest(new { message = "Error getting photos" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var photo = await _petPhotoService.GetByIdAsync(id);
            _logger.LogInformation($"Returned photo with id:{id}");

            return Ok(photo);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError($"Photo not found with id:{id}, error:{ex}");
            return NotFound(new { message = ex.Message });
        }
        catch (Exception exc)
        {
            _logger.LogError($"Error getting photo with id:{id}, error:{exc}");
            return BadRequest(new { message = "Error getting photo" });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _petPhotoService.RemovePhotoAsync(id);
            _logger.LogInformation($"Photo deleted: {id}");

            return Ok(new { message = "Photo deleted successfully" });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError($"Unauthorized access: {ex.Message}");
            return Unauthorized(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogError($"Photo not found with id:{id}, error:{ex}");
            return NotFound(new { message = ex.Message });
        }
        catch (Exception exc)
        {
            _logger.LogError($"Photo not deleted: {id}, error:{exc}");
            return BadRequest(new { message = "Photo not deleted" });
        }
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(int petId, IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file uploaded" });

            await _petPhotoService.AddPhotoAsync(petId, file.OpenReadStream(), file.FileName);
            _logger.LogInformation($"Photo uploaded for pet {petId}");

            return Ok(new { message = "Photo uploaded successfully" });
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
            _logger.LogError($"Photo not uploaded for pet {petId}, error:{exc}");
            return BadRequest(new { message = "Photo not uploaded" });
        }
    }
}