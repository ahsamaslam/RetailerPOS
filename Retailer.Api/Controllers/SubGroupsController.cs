using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Repositories;

namespace Retailer.POS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubGroupsController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    public SubGroupsController(IUnitOfWork uow) => _uow = uow;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var subGroups = await _uow.GetSubGroupsWithGroupAsync();
        return Ok(subGroups);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var subGroup = await _uow.GetSubGroupByIdWithGroupAsync(id);
        if (subGroup == null) return NotFound();
        return Ok(subGroup);
    }

    [HttpGet("bygroup/{groupId:int}")]
    public async Task<IActionResult> GetByGroup(int groupId)
    {
        var list = await _uow.ItemSubGroups.Query()
                     .Where(s => s.GroupId == groupId)
                     .ToListAsync();
        return Ok(list);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ItemSubGroup model)
    {
        await _uow.ItemSubGroups.AddAsync(model);
        await _uow.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ItemSubGroup model)
    {
        var existing = await _uow.ItemSubGroups.GetByIdAsync(id);
        if (existing == null) return NotFound();
        existing.Name = model.Name;
        existing.GroupId = model.GroupId;
        _uow.ItemSubGroups.Update(existing);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _uow.ItemSubGroups.GetByIdAsync(id);
        if (existing == null) return NotFound();
        _uow.ItemSubGroups.Remove(existing);
        await _uow.SaveChangesAsync();
        return NoContent();
    }
}
