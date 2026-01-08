using AuthModule.API.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Retailer.Api.Infrastructure;
using Retailer.POS.Api.DTOs;
using Retailer.POS.Api.Services;

namespace Retailer.POS.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ItemsController : ControllerBase
    {
        private readonly IItemService _svc;
        public ItemsController(IItemService svc) => _svc = svc;
        private Guid CompanyId => HttpContext.GetCompanyId();

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? term, [FromQuery] int take = 20)
            => Ok(await _svc.SearchAsync(CompanyId, term, take));

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _svc.GetAllAsync(CompanyId));

        [HttpGet("GetStockItemsAsync/{categoryId}/{groupId}")]
        public async Task<IActionResult> GetStockItemsAsync(int categoryId, int groupId)
        {
            return Ok(await _svc.GetStockItemsAsync(categoryId, groupId));

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var item = await _svc.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateItemDto dto)
        {
            try
            {
                var exists = await _svc.GetAllAsync(CompanyId)
             .ContinueWith(t => t.Result
             .Any(c => c.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase)));

                if (exists)
                    return Conflict(new { message = "Item Name already exists." });

                var created = await _svc.CreateAsync(dto, CompanyId);
                return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
            }
            catch (Exception exx)
            {
                return BadRequest(exx);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateItemDto dto)
        {
            await _svc.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _svc.DeleteAsync(id);
            return NoContent();
        }
    }

}