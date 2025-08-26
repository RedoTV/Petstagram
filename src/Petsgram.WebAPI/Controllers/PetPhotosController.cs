using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Petsgram.Application.DTOs.PetPhotos;
using Petsgram.Application.Interfaces.PetPhotos;

namespace Petsgram.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PetPhotosController : ControllerBase
{
    private readonly IPetPhotoService _petPhotoService;

    public PetPhotosController(IPetPhotoService petPhotoService)
    {
        _petPhotoService = petPhotoService;
    }

    [HttpGet("by-pet/{petId}")]
    public async Task<IActionResult> GetAllByPet(int petId, CancellationToken cancellationToken = default)
    {
        var photos = await _petPhotoService.GetAllByPetIdAsync(petId, cancellationToken);
        return Ok(photos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var photo = await _petPhotoService.GetByIdAsync(id, cancellationToken);
        return Ok(photo);
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] UploadPhotoRequest request, CancellationToken cancellationToken = default)
    {
        var uploaded = await _petPhotoService.AddPhotoAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = uploaded.Id }, uploaded);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await _petPhotoService.RemovePhotoAsync(id, cancellationToken);
        return Ok(deleted);
    }
}