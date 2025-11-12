using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Retailer.Api.DTOs;
using Retailer.Api.Entities;
using Retailer.POS.Api.DTOs;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Repositories;

namespace Retailer.POS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScopesController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public ScopesController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // GET: api/scopes
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var scopes = await _uow.Scopes.GetAllAsync();
            var dtos = scopes.Select(s => new ScopeDto { Id = s.Id, ScopeName = s.ScopeName }).ToList();
            return Ok(dtos);
        }

        // GET: api/scopes/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var scope = await _uow.Scopes.GetByIdAsync(id);
            if (scope == null) return NotFound();
            return Ok(new ScopeDto { Id = scope.Id, ScopeName = scope.ScopeName });
        }

        // POST: api/scopes
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ScopeDto dto)
        {
            if (dto == null) return BadRequest();

            var entity = new Scope
            {
                ScopeName = dto.ScopeName
            };

            await _uow.Scopes.AddAsync(entity);
            await _uow.SaveChangesAsync();

            var resultDto = new ScopeDto { Id = entity.Id, ScopeName = entity.ScopeName };
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, resultDto);
        }

        // PUT: api/scopes/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ScopeDto dto)
        {
            if (dto == null || id != dto.Id) return BadRequest();

            var entity = await _uow.Scopes.GetByIdAsync(id);
            if (entity == null) return NotFound();

            entity.ScopeName = dto.ScopeName;
            _uow.Scopes.Update(entity);
            await _uow.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/scopes/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _uow.Scopes.GetByIdAsync(id);
            if (entity == null) return NotFound();

            _uow.Scopes.Remove(entity);
            await _uow.SaveChangesAsync();
            return NoContent();
        }
    }
}
