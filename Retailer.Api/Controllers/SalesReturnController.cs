using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Retailer.Api.DTOs;
using Retailer.Api.Infrastructure;
using Retailer.Api.Migrations;
using Retailer.Api.Services; // optional DTO namespace if you have
using Retailer.POS.Api.Data;
using Retailer.POS.Api.DTOs;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Repositories; // your IUnitOfWork namespace

namespace Retailer.POS.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SalesReturnController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IFbrClient _fbrClient;
        private readonly ICompanyService _companyService;
        private readonly RetailerDbContext _context;
        public SalesReturnController(IUnitOfWork uow, IFbrClient fbrClient, ICompanyService companyService, RetailerDbContext context)
        {
            _uow = uow;
            _fbrClient = fbrClient;
            _companyService = companyService;
            _context = context;
        }
        private Guid CompanyId => HttpContext.GetCompanyId();
        private LoginDto CurrentUser => HttpContext.GetUserId();

        [HttpGet]
        [HttpGet("GetAllDateWise/{sdate}/{edate}")]
        public async Task<IActionResult> GetAllDateWise(DateTime sdate, DateTime edate)
        {
            // Use repository Query() if available; otherwise GetAllAsync and include details via DB context.
            var list = await _uow.SalesReturnMaster

                .Query()
                .Where(r => r.CompanyId == CompanyId && r.Date.Date >= sdate.Date.Date && r.Date.Date <= edate.Date && r.Active)
                //.Include(s => s.Details)
                .OrderByDescending(s => s.Date)
                .ToListAsync();

            return Ok(list);
        }

        // GET api/sales/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var master = await _uow.SalesReturnMaster
                .Query()
                .Include(s => s.Details)
                .Where(s => s.Id == id  && s.Active)
                .Select(s => new SalesReturnMasterDto
                {
                    Id = s.Id,
                    Date = s.Date,
                    UserId = CurrentUser.Id,
                    UserName = CurrentUser.UserName,
                    BranchId = s.BranchId,
                    CustomerName = s.CustomerName,
                    SaleType = s.SaleType,
                    SubTotal = s.SubTotal,
                    TotalDiscount = s.TotalDiscount,
                    TaxAmount = s.TaxAmount,
                    BalanceAmount = s.BalanceAmount,
                    CustomerCode = s.CustomerCode,
                    Details = s.Details.Select(d => new SalesReturnDetailDto
                    {
                        Id = d.Id,
                        ItemCode = d.ItemCode,
                        ItemName = d.ItemName,
                        Rate = d.Rate,
                        Qty = d.Qty,
                        Discount = d.Discount,
                        TaxPercentage = d.TaxPercentage,
                        TaxAmount = d.TaxAmount,
                        Amount = d.Amount
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (master == null) return NotFound();

            return Ok(master);
        }

        // POST api/sales
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SalesReturnMaster model)
        {
            try
            {
                if (model == null) return BadRequest();

                // ensure details' SalesMaster navigation is cleared (EF will set it)
                foreach (var d in model.Details)
                {
                    // Reset IDs to ensure EF treats them as new (if client accidentally sent Ids)
                    d.Id = 0;
                    // Ensure FK is not set to an incorrect value
                    d.SalesReturnMasterId = 0;
                    // Ensure navigation property points to parent (optional)
                    d.SalesReturnMaster = model;
                }
                //foreach (var d in model.Details) d.SalesMaster = null;
                model.CompanyId = CompanyId;
                model.Active = true;    
                var customer = await _context.Customers
             .Where(c => c.Id == model.CustomerCode)
              
             .FirstOrDefaultAsync();

                if (customer.Name != null)
                {
                    model.CustomerName = customer.Name; // Assuming SaleMaster has CustomerName property
                }

                model.CreateDate = DateTime.UtcNow;
                model.totalAmount =  model.BalanceAmount;
                model.UserId = CurrentUser.Id;
                await _uow.SalesReturnMaster.AddAsync(model);
                await _uow.SaveChangesAsync();
                var itemids = model.Details.GroupBy(x => x.ItemCode).Select(x => x.Key).ToList();
                var year = model.Year;
                var companyIdClaim = User.FindFirst("companyId")?.Value;
                if (!string.IsNullOrEmpty(companyIdClaim) && Guid.TryParse(companyIdClaim, out var companyId))
                {
                    var company = await _companyService.GetCompanyByIdAsync(Guid.Parse(companyIdClaim) );
                   
                }
                else
                {
                    //_logger.LogDebug("No companyId claim present; skipping FBR send");
                }
            var ledgerService = new CustomerLedgerService(_context);
                await ledgerService.PostLedgerAsync(model);
                await _uow.UpdateQtys(itemids, year);

                return Ok(model);
            }
            catch (Exception exx)
            {
                return BadRequest(exx.StackTrace);

            }
        }

        // PUT api/sales/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] SalesMaster model)
        {
            if (model == null || id != model.Id) return BadRequest();

            var existing = await _uow.SalesReturnMaster.Query()
                .Include(s => s.Details)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (existing == null) return NotFound();
            var customer = await _context.Customers
              .Where(c => c.Id == model.CustomerCode)
              .Select(c => new { c.Name })
              .FirstOrDefaultAsync();

            if (customer != null)
            {
                existing.CustomerName = customer.Name; // Assuming SaleMaster has CustomerName property
            }
            existing.totalAmount = model.SubTotal + model.TaxAmount - model.TotalDiscount; ;
            // update scalar properties
            existing.Active = true;
            existing.Date = model.Date;
            existing.UserId = CurrentUser.Id;
            existing.BranchId = model.BranchId;
            existing.CustomerName = model.CustomerName;
            existing.SubTotal = model.SubTotal;
            existing.TotalDiscount = model.TotalDiscount;
            existing.TaxAmount = model.TaxAmount;
            existing.BalanceAmount = model.BalanceAmount;
           // existing.CustomerCode = model.CustomerCode;
            existing.CustomerName = model.CustomerName;
            // --- synchronize details ---
            // remove details not present
            var toRemove = existing.Details.Where(ed => !model.Details.Any(d => d.Id == ed.Id)).ToList();
            foreach (var rem in toRemove)
                _uow.SalesReturnDetails.Remove(rem);

            // update or add details
            foreach (var d in model.Details)
            {
                if (d.Id > 0)
                {
                    var existDetail = existing.Details.FirstOrDefault(x => x.Id == d.Id);
                    if (existDetail != null)
                    {
                        existDetail.ItemCode = d.ItemCode;
                        existDetail.ItemName = d.ItemName;
                        existDetail.Rate = d.Rate;
                        existDetail.Qty = d.Qty;
                        existDetail.CompanyId = existing.CompanyId;
                        existDetail.Discount = d.Discount;
                        existDetail.TaxPercentage = d.TaxPercentage;
                        existDetail.TaxAmount = d.TaxAmount;
                        existDetail.Amount = d.Amount;
                        _uow.SalesReturnDetails.Update(existDetail);
                    }
                }
                else
                {
                    // new detail
                    var newDetail = new SalesDetail
                    {
                        ItemCode = d.ItemCode,
                        ItemName = d.ItemName,
                        Rate = d.Rate,
                        Qty = d.Qty,
                        Discount = d.Discount,
                        TaxPercentage = d.TaxPercentage,
                        TaxAmount = d.TaxAmount,
                        Amount = d.Amount,
                        SalesMasterId = existing.Id
                    };
                    await _uow.SalesDetails.AddAsync(newDetail);
                }
            }

            _uow.SalesReturnMaster.Update(existing);
            await _uow.SaveChangesAsync();
            var ledgerService = new CustomerLedgerService(_context);
            await ledgerService.UpdateLedgerAsync(existing);
            return NoContent();
        }

        // DELETE api/sales/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _uow.SalesReturnMaster.GetAsync(b => b.Id == id);
            if (existing == null) return NotFound();
            existing.Active = false;
            _uow.SalesReturnMaster.Update(existing);

            await _uow.SaveChangesAsync();
            var ledgerService = new CustomerLedgerService(_context);
            await ledgerService.ReverseLedgerAsync(existing);
            return NoContent();
        }
    }
}
