using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Retailer.Api.Entities;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Repositories;

namespace Retailer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        public CitiesController(IUnitOfWork uow) => _uow = uow;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _uow.Cities.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var entity = await _uow.Cities.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Cities model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                return BadRequest(new { message = "Name is required." });

            // 🔍 Check if name already exists (case-insensitive)
            var exists = await _uow.Cities.GetAllAsync()
                .ContinueWith(t => t.Result
                .Any(c => c.Name.Equals(model.Name, StringComparison.OrdinalIgnoreCase)));
            
            if (exists)
                return Conflict(new { message = "City name already exists." });

            await _uow.Cities.AddAsync(model);
            await _uow.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int Id, [FromBody] ItemCategory model)
        {
            var existing = await _uow.Cities.GetByIdAsync(Id);
            if (existing == null) return NotFound();
            existing.Name = model.Name;
            _uow.Cities.Update(existing);
            await _uow.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _uow.Cities.GetByIdAsync(id);
            if (existing == null) return NotFound();
            _uow.Cities.Remove(existing);
            await _uow.SaveChangesAsync();
            return NoContent();
        }
    }
}
