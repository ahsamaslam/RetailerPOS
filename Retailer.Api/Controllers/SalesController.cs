using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Repositories; // your IUnitOfWork namespace
using Retailer.POS.Api.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Retailer.Api.DTOs; // optional DTO namespace if you have

namespace Retailer.POS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public SalesController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // GET api/sales
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Use repository Query() if available; otherwise GetAllAsync and include details via DB context.
            var list = await _uow.SalesMasters
                .Query()
                //.Include(s => s.Details)
                .OrderByDescending(s => s.Date)
                .ToListAsync();

            return Ok(list);
        }

        // GET api/sales/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var master = await _uow.SalesMasters
                .Query()
                .Include(s => s.Details)
                .Where(s => s.Id == id)
                .Select(s => new SalesMasterDto
                {
                    Id = s.Id,
                    Date = s.Date,
                    LoginId = s.LoginId,
                    BranchId = s.BranchId,
                    CustomerName = s.CustomerName,
                    SubTotal = s.SubTotal,
                    TotalDiscount = s.TotalDiscount,
                    TaxAmount = s.TaxAmount,
                    BalanceAmount = s.BalanceAmount,
                    CustomerCode = s.CustomerCode,
                    Details = s.Details.Select(d => new SalesDetailDto
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
        public async Task<IActionResult> Create([FromBody] SalesMaster model)
        {
            if (model == null) return BadRequest();

            // ensure details' SalesMaster navigation is cleared (EF will set it)
            foreach (var d in model.Details)
            {
                // Reset IDs to ensure EF treats them as new (if client accidentally sent Ids)
                d.Id = 0;
                // Ensure FK is not set to an incorrect value
                d.SalesMasterId = 0;
                // Ensure navigation property points to parent (optional)
                d.SalesMaster = model;
            }
            //foreach (var d in model.Details) d.SalesMaster = null;

            await _uow.SalesMasters.AddAsync(model);
            await _uow.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
        }

        // PUT api/sales/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] SalesMaster model)
        {
            if (model == null || id != model.Id) return BadRequest();

            var existing = await _uow.SalesMasters.Query()
                .Include(s => s.Details)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (existing == null) return NotFound();

            // update scalar properties
            existing.Date = model.Date;
            existing.LoginId = model.LoginId;
            existing.BranchId = model.BranchId;
            existing.CustomerName = model.CustomerName;
            existing.SubTotal = model.SubTotal;
            existing.TotalDiscount = model.TotalDiscount;
            existing.TaxAmount = model.TaxAmount;
            existing.BalanceAmount = model.BalanceAmount;
            existing.CustomerCode = model.CustomerCode;

            // --- synchronize details ---
            // remove details not present
            var toRemove = existing.Details.Where(ed => !model.Details.Any(d => d.Id == ed.Id)).ToList();
            foreach (var rem in toRemove)
                _uow.SalesDetails.Remove(rem);

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
                        existDetail.Discount = d.Discount;
                        existDetail.TaxPercentage = d.TaxPercentage;
                        existDetail.TaxAmount = d.TaxAmount;
                        existDetail.Amount = d.Amount;
                        _uow.SalesDetails.Update(existDetail);
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

            _uow.SalesMasters.Update(existing);
            await _uow.SaveChangesAsync();
            return NoContent();
        }

        // DELETE api/sales/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _uow.SalesMasters.GetByIdAsync(id);
            if (existing == null) return NotFound();

            _uow.SalesMasters.Remove(existing);
            await _uow.SaveChangesAsync();
            return NoContent();
        }
    }
}
