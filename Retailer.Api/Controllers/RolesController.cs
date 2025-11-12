using Microsoft.AspNetCore.Mvc;
using Retailer.Api.DTOs;
using Retailer.Api.Entities;
using Retailer.POS.Api.Repositories;

namespace Retailer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        public RolesController(IUnitOfWork uow) => _uow = uow;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _uow.Roles.GetAllAsync());

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RoleDto dto)
        {
            var role = new Role { RoleName = dto.RoleName };
            await _uow.Roles.AddAsync(role);
            await _uow.SaveChangesAsync();

            // Assign scopes
            foreach (var scopeId in dto.ScopeIds)
            {
                await _uow.RoleScopes.AddAsync(new RoleScope { RoleId = role.Id, ScopeId = scopeId });
            }
            await _uow.SaveChangesAsync();
            return Ok(role);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] RoleDto dto)
        {
            var role = await _uow.Roles.GetByIdAsync(id);
            if (role == null) return NotFound();
            role.RoleName = dto.RoleName;

            // Update scopes
            var existingScopes = (await _uow.RoleScopes.GetAllAsync()).Where(rs => rs.RoleId == id).ToList();
            foreach (var rs in existingScopes) _uow.RoleScopes.Remove(rs);

            foreach (var scopeId in dto.ScopeIds)
            {
                await _uow.RoleScopes.AddAsync(new RoleScope { RoleId = id, ScopeId = scopeId });
            }
            await _uow.SaveChangesAsync();
            return Ok(role);
        }
    }

}
