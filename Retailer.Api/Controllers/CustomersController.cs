using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Retailer.Api.Entities.Ledger;
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
    public class CustomersController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly RetailerDbContext _context;
        public CustomersController(IUnitOfWork uow, RetailerDbContext context)
        {
            _uow = uow;
            _context = context;
        }

        private Guid CompanyId => HttpContext.GetCompanyId();

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _uow.Customers.GetAllAsync(b => b.CompanyId == CompanyId));

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var c = await _uow.Customers.GetAsync(b => b.Id == id);
            if (c == null) return NotFound();
            return Ok(c);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Customer model)
        {
            model.CompanyId = CompanyId;    
            await _uow.Customers.AddAsync(model);
            await _uow.SaveChangesAsync();
            var ledgerService = new CustomerLedgerService(_context);
            await ledgerService.PostLedgerAsync(model);
            return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
        }
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int Id, [FromBody] Customer model)
        { 
                var existing = await _uow.Customers.GetAsync(b => b.Id == Id);
                if (existing == null) return NotFound();
                existing.openDate =  model.openDate;
                existing.Phone = model.Phone; 
                 existing.Address = model.Address;  
                    existing.CityId = model.CityId;
                existing.STRN = model.STRN;
                existing.CNIC = model.CNIC;
                existing.NTN = model.NTN;
                existing.openDate = model.openDate;
                existing.openingBalance = model.openingBalance;
                 existing.Mobile =model.Mobile;
                 existing.Phone =model.Phone;
                 existing.Register =model.Register;
                _uow.Customers.Update(existing);
                await _uow.SaveChangesAsync();
            var ledgerService = new CustomerLedgerService(_context);
            await ledgerService.UpdateLedgerAsync(model);

            return NoContent();
        }
    }
}
