using Microsoft.AspNetCore.Mvc;
using Retailer.Api.Entities;
using Retailer.POS.Api.DTOs;
using Retailer.POS.Api.Repositories;
using Retailer.POS.Api.Services;

namespace Retailer.POS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProvienceController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        public ProvienceController(IUnitOfWork uow) => _uow = uow;


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _uow.Proviences.GetAllAsync();
            return Ok(list);
        }



        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Provience model)
        { 
                if (string.IsNullOrWhiteSpace(model.Name))
                    return BadRequest(new { message = "Name is required." });

                // 🔍 Check if name already exists (case-insensitive)
                var exists = await _uow.Cities.GetAllAsync()
                    .ContinueWith(t => t.Result
                    .Any(c => c.Name.Equals(model.Name, StringComparison.OrdinalIgnoreCase)));

                if (exists)
                    return Conflict(new { message = "City name already exists." });

                await _uow.Proviences.AddAsync(model);
                await _uow.SaveChangesAsync();

                return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
            
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var entity = await _uow.Proviences.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return Ok(entity);
        }
    }

}

