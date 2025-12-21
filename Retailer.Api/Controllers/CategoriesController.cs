using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Retailer.Api.Infrastructure;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Repositories;

namespace Retailer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        public CategoriesController(IUnitOfWork uow) => _uow = uow;
        private Guid CompanyId => HttpContext.GetCompanyId();


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _uow.ItemCategories.GetAllAsync(b => b.CompanyId == CompanyId);
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var entity = await _uow.ItemCategories.GetAsync(b => b.Id == id);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ItemCategory model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                return BadRequest(new { message = "Name is required." });

            // 🔍 Check if name already exists (case-insensitive)
            var exists = await _uow.ItemCategories.GetAllAsync()
                .ContinueWith(t => t.Result
                .Any(c => c.Name.Equals(model.Name, StringComparison.OrdinalIgnoreCase)));
            
            if (exists)
                return Conflict(new { message = "Category name already exists." });

            model.CompanyId = CompanyId;    
            await _uow.ItemCategories.AddAsync(model);
            await _uow.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ItemCategory model)
        {
            var existing = await _uow.ItemCategories.GetAsync(b => b.Id == id);
            if (existing == null) return NotFound();
            existing.Name = model.Name;
            _uow.ItemCategories.Update(existing);
            await _uow.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _uow.ItemCategories.GetAsync(b => b.Id == id);
            if (existing == null) return NotFound();
            _uow.ItemCategories.Remove(existing);
            await _uow.SaveChangesAsync();
            return NoContent();
        }
    }
}
