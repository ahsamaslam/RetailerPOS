using Microsoft.AspNetCore.Mvc;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Repositories;

namespace Retailer.POS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupsController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    public GroupsController(IUnitOfWork uow) => _uow = uow;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _uow.ItemGroups.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var entity = await _uow.ItemGroups.GetByIdAsync(id);
        if (entity == null) return NotFound();
        return Ok(entity);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ItemGroup model)
    {
        await _uow.ItemGroups.AddAsync(model);
        await _uow.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ItemGroup model)
    {
        var existing = await _uow.ItemGroups.GetByIdAsync(id);
        if (existing == null) return NotFound();
        existing.Name = model.Name;
        _uow.ItemGroups.Update(existing);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _uow.ItemGroups.GetByIdAsync(id);
        if (existing == null) return NotFound();
        _uow.ItemGroups.Remove(existing);
        await _uow.SaveChangesAsync();
        return NoContent();
    }
}
