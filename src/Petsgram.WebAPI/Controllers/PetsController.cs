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
    private readonly IPetService _petService;

    public PetsController(IPetService petService)
    {
        _petService = petService;
    }

    [HttpGet("my-pets")]
    public async Task<IActionResult> GetCurrentUserPets(
        [FromQuery] string? currency = null,
        CancellationToken cancellationToken = default)
    {
        var pets = await _petService.GetCurrentUserPetsAsync(currency, cancellationToken);
        return Ok(pets);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetAllByUser(
        int userId, 
        [FromQuery] string? currency = null,
        CancellationToken cancellationToken = default)
    {
        var pets = await _petService.GetUserPetsAsync(userId, currency, cancellationToken);
        return Ok(pets);
    }

    [HttpGet("{petId}")]
    public async Task<IActionResult> GetById(
        int petId, 
        [FromQuery] string? currency = null,
        CancellationToken cancellationToken = default)
    {
        var pet = await _petService.GetPetByIdAsync(petId, currency, cancellationToken);
        return Ok(pet);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePetRequest request, CancellationToken cancellationToken = default)
    {
        var created = await _petService.AddPetToCurrentUserAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { petId = created.Id }, created);
    }

    [HttpPut("{petId}")]
    public async Task<IActionResult> Update(int petId, [FromBody] UpdatePetRequest request, CancellationToken cancellationToken = default)
    {
        var updated = await _petService.UpdatePetAsync(petId, request, cancellationToken);
        return Ok(updated);
    }

    [HttpDelete("{petId}")]
    public async Task<IActionResult> Delete(int petId, CancellationToken cancellationToken = default)
    {
        var deleted = await _petService.RemovePetAsync(petId, cancellationToken);
        return Ok(deleted);
    }
}
