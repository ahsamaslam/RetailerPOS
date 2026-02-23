using AuthModule.API.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Retailer.Api.Infrastructure;
using Retailer.POS.Api.DTOs;
using Retailer.POS.Api.Services;
using System.Text;

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
        public async Task<IActionResult> Search([FromQuery] int catd,  [FromQuery] string? term,  [FromQuery] int take = 20)
            => Ok(await _svc.SearchAsync(CompanyId, catd ,term, take));

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
        [HttpGet("export-csv")]
        public async Task<IActionResult> ExportItemsToCsv()
        {
            var items = await _svc.GetAllAsync(CompanyId);
                items
                .Select(i => new
                {
                    i.Barcode,
                    i.Name,
                    i.CategoryName
                    ,i.ItemTypeName
                    ,i.GroupName
                    , i.SubGroupName
                    
                   
                })
                .ToList();

            var csv = new StringBuilder();

            // Header
            csv.AppendLine(
                "Barcode,ItemName,CategoryName,ItemTypeName,GroupName,SubGroupName,Qty"
            );

            // Rows
            foreach (var item in items)
            {
                csv.AppendLine(
                    $"{_svc.Escape(item.Barcode)}," +
        $"{_svc.Escape(item.Name)}," +
        $"{_svc.Escape(item.CategoryName)}," +
        $"{_svc.Escape(item.ItemTypeName)}," +
        $"{_svc.Escape(item.GroupName)}," +
        $"{_svc.Escape(item.SubGroupName)}," +
        $"{_svc.Escape(Convert.ToString(item.QtyInHand))}"
    );
            }

            return File(
                Encoding.UTF8.GetBytes(csv.ToString()),
                "text/csv",
                $"Items_{DateTime.UtcNow:yyyyMMddHHmmss}.csv"
            );
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