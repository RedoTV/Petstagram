using Microsoft.AspNetCore.Mvc;
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
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var types = await _petTypeService.GetAllAsync();
            _logger.LogInformation($"Returned {types.Count()} pet types");
            return Ok(types);
        }
        catch (Exception exc)
        {
            _logger.LogError($"Error getting pet types: {exc}");
            return BadRequest(new { message = "Error getting pet types" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var type = await _petTypeService.GetByIdAsync(id);
            _logger.LogInformation($"Returned pet type with id:{id}");
            return Ok(type);
        }
        catch (Exception exc)
        {
            _logger.LogError($"Pet type not found with id:{id}, error:{exc}");
            return BadRequest(new { message = "Pet type not found" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromQuery] string name)
    {
        try
        {
            await _petTypeService.AddTypeAsync(name);
            _logger.LogInformation($"Pet type created: {name}");
            return Ok();
        }
        catch (Exception exc)
        {
            _logger.LogError($"Pet type not created, error:{exc}");
            return BadRequest(new { message = "Pet type not created" });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromQuery] string name)
    {
        try
        {
            await _petTypeService.UpdateTypeAsync(id, name);
            _logger.LogInformation($"Pet type updated: {id}");
            return Ok();
        }
        catch (Exception exc)
        {
            _logger.LogError($"Pet type not updated: {id}, error:{exc}");
            return BadRequest(new { message = "Pet type not updated" });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _petTypeService.RemoveTypeAsync(id);
            _logger.LogInformation($"Pet type deleted: {id}");
            return Ok();
        }
        catch (Exception exc)
        {
            _logger.LogError($"Pet type not deleted: {id}, error:{exc}");
            return BadRequest(new { message = "Pet type not deleted" });
        }
    }
}