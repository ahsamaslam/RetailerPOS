using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Retailer.Api.DTOs;
using Retailer.Api.Entities;
using Retailer.Api.Infrastructure;
using Retailer.Api.Services;
using Retailer.POS.Api.Data;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Repositories;

namespace Retailer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class CustomerPaymentController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly RetailerDbContext _context;
        public CustomerPaymentController(IUnitOfWork uow, RetailerDbContext context)
        {
            _uow = uow; 
            _context = context;
        }
        

        private Guid CompanyId => HttpContext.GetCompanyId();

        [HttpGet]
        public async Task<IActionResult> GetAll(Guid companyID) =>
            Ok(await _uow.CustomerPayment.GetAllAsync(b => b.companyId == companyID));

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var c = await _uow.CustomerPayment.GetAsync(b => b.Id == id );
            if (c == null) return NotFound();
            return Ok(c);
        }
        [HttpGet("GetAllDateWise/{sdate}/{edate}")]
        public async Task<IActionResult> Get(DateTime sdate , DateTime edate)
        {
            try
            {
                var c = await _uow.CustomerPayment.Query()
                    .Include(b => b.Customer)
                    .Include(b => b.PaymentMethod)
                    .Include(b => b.Bank)
                    .Where(b => b.PaymentDate >= sdate && b.PaymentDate <= edate && b.companyId == CompanyId  && b.status==1).ToListAsync();
                if (c == null) return NotFound();
                return Ok(c);
            }
            catch (Exception exx)
            { return BadRequest(exx.Message); }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CustomerPayment model)
        {
            try
            {
                model.status = 1;   
                model.companyId = CompanyId;
                await _uow.CustomerPayment.AddAsync(model);
                await _uow.SaveChangesAsync();
                var ledgerService = new CustomerLedgerService(_context);
                await ledgerService.PostLedgerAsync(model);
                return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
            }
            catch (Exception ex)
            { 
            return BadRequest(ex.Message);  
            }
        }
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int Id, [FromBody] CustomerPayment model)
        {  
            var existing = await _uow.CustomerPayment.GetAsync(p => p.Id == model.Id);
            if (existing == null)
                return NotFound();

            existing.CustomerId = model.CustomerId;
            existing.Type = model.Type;
            existing.Amount = model.Amount;
            existing.PaymentDate = model.PaymentDate;
            existing.PaymentMethodId = model.PaymentMethodId;
            existing.status = 1;
            existing.BankId = model.BankId;
            existing.bankName = model.bankName;

            existing.taxPercent = model.taxPercent;
            existing.taxAmount = model.taxAmount;

            existing.whtPercent = model.whtPercent;
            existing.whtAmount = model.whtAmount;
              

            _uow.CustomerPayment.Update(existing);
            await _uow.SaveChangesAsync();
            var ledgerService = new CustomerLedgerService(_context);
            await ledgerService.UpdateLedgerAsync(model);


            return NoContent();
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var existing = await _uow.CustomerPayment.GetAsync(p => p.Id == id);
            if (existing == null)
                return NotFound();

            // ✅ Mark as deleted
            existing.status = 0;

            _uow.CustomerPayment.Update(existing);
            await _uow.SaveChangesAsync();
            var ledgerService = new CustomerLedgerService(_context);
            await ledgerService.ReverseLedgerAsync(existing);
            return NoContent();
        }

    }
}
