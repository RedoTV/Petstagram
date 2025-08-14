using Microsoft.AspNetCore.Mvc;
using Petsgram.Application.DTOs.PetTypes;
using Petsgram.Application.Interfaces.PetTypes;

namespace Petsgram.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PetTypesController : ControllerBase
{
    private readonly ILogger<PetTypesController> _logger;
    private readonly IPetTypeService _petTypeService;
    public PetTypesController(ILogger<PetTypesController> logger, IPetTypeService petTypeService)
    {
        _logger = logger;
        _petTypeService = petTypeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        var types = await _petTypeService.GetAllAsync(cancellationToken);
        return Ok(types);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var type = await _petTypeService.GetByIdAsync(id, cancellationToken);
        return Ok(type);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PetTypeCreateRequest request, CancellationToken cancellationToken)
    {
        var created = await _petTypeService.AddTypeAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] PetTypeUpdateRequest request, CancellationToken cancellationToken)
    {
        var updated = await _petTypeService.UpdateTypeAsync(id, request, cancellationToken);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _petTypeService.RemoveTypeAsync(id, cancellationToken);
        return Ok(deleted);
    }
}