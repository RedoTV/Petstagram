using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Petsgram.Application.Interfaces.PetPhotos;

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
    public async Task<IActionResult> GetAllByPet(int petId, CancellationToken cancellationToken = default)
    {
        try
        {
            var photos = await _petPhotoService.GetAllByPetIdAsync(petId, cancellationToken);
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
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var photo = await _petPhotoService.GetByIdAsync(id, cancellationToken);
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
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _petPhotoService.RemovePhotoAsync(id, cancellationToken);
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
    public async Task<IActionResult> Upload(int petId, int userId, IFormFile file, CancellationToken cancellationToken = default)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            using var stream = file.OpenReadStream();
            await _petPhotoService.AddPhotoAsync(petId, userId, stream, file.FileName, cancellationToken);
            _logger.LogInformation($"Photo uploaded for pet {petId}, user {userId}");

            return Ok(new { message = $"Photo uploaded for pet {petId}, user {userId}" });
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
            _logger.LogError($"Photo not uploaded for pet {petId}, user {userId}, error:{exc}");
            return BadRequest(new { message = "Photo not uploaded" });
        }
    }
}