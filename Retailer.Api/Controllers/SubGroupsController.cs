using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Retailer.Api.DTOs;
using Retailer.Api.Infrastructure;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Repositories;

namespace Retailer.POS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubGroupsController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    public SubGroupsController(IUnitOfWork uow) => _uow = uow;
    private Guid CompanyId => HttpContext.GetCompanyId();

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var subGroups = await _uow.ItemSubGroups.Query()
                .Include(sg => sg.Group) // Include navigation property
                .Where(x => x.CompanyId == CompanyId)
                .Select(sg => new ItemSubGroupDto
                {
                    Id = sg.Id,
                    Name = sg.Name,
                    GroupId = sg.GroupId,
                    GroupName = sg.Group != null ? sg.Group.Name : null
                })
                .ToListAsync();
        return Ok(subGroups);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {

        var subGroup = await _uow.ItemSubGroups.Query()
                .Include(sg => sg.Group) // Include related Group
                .Where(sg => sg.Id == id)
                .Select(sg => new ItemSubGroupDto
                {
                    Id = sg.Id,
                    Name = sg.Name,
                    GroupId = sg.GroupId,
                    GroupName = sg.Group != null ? sg.Group.Name : null
                })
                .FirstOrDefaultAsync();
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
        var exists = await _uow.ItemSubGroups.GetAllAsync(b => b.CompanyId == CompanyId)
          .ContinueWith(t => t.Result
          .Any(c => c.Name.Equals(model.Name, StringComparison.OrdinalIgnoreCase)));

        if (exists)
            return Conflict(new { message = "Sub Groups already exists." });

        model.CompanyId = CompanyId;
        await _uow.ItemSubGroups.AddAsync(model);
        await _uow.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ItemSubGroup model)
    {
        var existing = await _uow.ItemSubGroups.GetAsync(b => b.Id == id);
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
        var existing = await _uow.ItemSubGroups.GetAsync(b => b.Id == id);
        if (existing == null) return NotFound();
        _uow.ItemSubGroups.Remove(existing);
        await _uow.SaveChangesAsync();
        return NoContent();
    }
}
