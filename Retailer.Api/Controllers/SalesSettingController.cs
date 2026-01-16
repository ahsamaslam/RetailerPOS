using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Retailer.Api.DataSet;
using Retailer.Api.DTOs;
using Retailer.Api.Entities;
using Retailer.Api.Infrastructure;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Repositories;
using Retailer.POS.Api.Services; // your IUnitOfWork namespace

namespace Retailer.POS.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SalesSettingController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        public SalesSettingController(IUnitOfWork uow) => _uow = uow;


        private Guid CompanyId => HttpContext.GetCompanyId();  
        
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
            => 
            Ok(await _uow.SaleInvoiceSettings.Query().Where( r=>r.branchID== id && r.companyID== CompanyId).FirstOrDefaultAsync());

        [HttpGet("byID/{id:int}")]
        public async Task<IActionResult> GetbyID(int id)
        {
            var setting = await _uow.SaleInvoiceSettings
                .Query()
                .Where(r => r.Id == id && r.companyID == CompanyId)
                .FirstOrDefaultAsync();

            if (setting == null)
                return NotFound();

            return Ok(setting);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
                  =>
                  Ok(await _uow.SaleInvoiceSettings.Query().Where(r => r.companyID == CompanyId).ToListAsync());
        // Create
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SaleInvoiceSettings setting)
        {
            setting.companyID = CompanyId;
            setting.CreatedAt = DateTime.UtcNow;

            await _uow.SaleInvoiceSettings.AddAsync(setting);
            await _uow.SaveChangesAsync();

            return CreatedAtAction(nameof(GetbyID), new { id = setting.Id }, setting);
        }

        // Update
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] SaleInvoiceSettings updated)
        {
            var setting = await _uow.SaleInvoiceSettings
                .Query()
                .Where(r => r.Id == id && r.companyID == CompanyId)
                .FirstOrDefaultAsync();

            if (setting == null)
                return NotFound();

            // Copy values
            _uow.SaleInvoiceSettings.Update(updated);
            await _uow.SaveChangesAsync();

            return NoContent();
        }

        // Delete
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var setting = await _uow.SaleInvoiceSettings
                .Query()
                .Where(r => r.Id == id && r.companyID == CompanyId)
                .FirstOrDefaultAsync();

            if (setting == null)
                return NotFound();

            _uow.SaleInvoiceSettings.Remove(setting);
            await _uow.SaveChangesAsync();

            return NoContent();
        }
    }
}
