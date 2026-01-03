using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Retailer.Api.Infrastructure;
using Retailer.Api.Services;
using Retailer.POS.Api.Data;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Repositories;

namespace Retailer.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VendorsController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly RetailerDbContext _context;
        public VendorsController(IUnitOfWork uow,RetailerDbContext context) {_uow = uow; _context=context; }    
        private Guid CompanyId => HttpContext.GetCompanyId();

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _uow.Vendors.GetAllAsync(x => x.CompanyId == CompanyId));

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var v = await _uow.Vendors.GetAsync(b => b.Id == id);
            if (v == null) return NotFound();
            return Ok(v);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Vendor model)
        {
            model.CompanyId = CompanyId;
            await _uow.Vendors.AddAsync(model);
            await _uow.SaveChangesAsync();
			var ledgerService = new VendorLedgerService(_context);
			await ledgerService.PostLedgerAsync(model);
			return CreatedAtAction(nameof(Get), new { id = model.Id }, model);

        }
		[HttpPut("{id:int}")]
		public async Task<IActionResult> Update(int Id, [FromBody] Vendor model)
		{
			var existing = await _uow.Vendors.GetAsync(b => b.Id == Id);
			if (existing == null) return NotFound();
			existing.openDate = model.openDate;
			existing.Phone = model.Phone;
			existing.Address = model.Address;
			existing.CityId = model.CityId;
			existing.STRN = model.STRN;
			existing.CNIC = model.CNIC;
			existing.NTN = model.NTN;
			existing.openDate = model.openDate;
			existing.openingBalance = model.openingBalance;
			existing.Mobile = model.Mobile;
			existing.Phone = model.Phone;
			existing.Register = model.Register;
			_uow.Vendors.Update(existing);
			await _uow.SaveChangesAsync();
			var ledgerService = new VendorLedgerService(_context);
			await ledgerService.UpdateLedgerAsync(existing);

			return NoContent();
		}
	}
}