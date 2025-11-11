using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Repositories;

namespace Retailer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemTypeController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        public ItemTypeController(IUnitOfWork uow) => _uow = uow;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _uow.ItemTypes.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var entity = await _uow.ItemTypes.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ItemType model)
        {
            await _uow.ItemTypes.AddAsync(model);
            await _uow.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ItemCategory model)
        {
            var existing = await _uow.ItemTypes.GetByIdAsync(id);
            if (existing == null) return NotFound();
            existing.Name = model.Name;
            _uow.ItemTypes.Update(existing);
            await _uow.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _uow.ItemTypes.GetByIdAsync(id);
            if (existing == null) return NotFound();
            _uow.ItemTypes.Remove(existing);
            await _uow.SaveChangesAsync();
            return NoContent();
        }
    }
}
