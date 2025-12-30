using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Retailer.Api.DTOs;
using Retailer.Api.Infrastructure;
using Retailer.Api.Services;
using Retailer.POS.Api.Data;
using Retailer.POS.Api.DTOs;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Services;

namespace Retailer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PurchasesController : ControllerBase
    {
        private readonly IPurchaseService _svc;
        private readonly RetailerDbContext _context;
        public PurchasesController(IPurchaseService svc, RetailerDbContext context) { _svc = svc; _context = context; }
        private Guid CompanyId => HttpContext.GetCompanyId();
        private LoginDto CurrentUser => HttpContext.GetUserId();

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Use repository Query() if available; otherwise GetAllAsync and include details via DB context.
            var list = await _svc.GetAll(CompanyId);
            return Ok(list);
        }



        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePurchaseDto dto)
        {
            try
            {

                var created = await _svc.CreatePurchaseAsync(dto, CompanyId, CurrentUser.Id);
                var itemids = created.Details.GroupBy(x => x.ItemId).Select(x => x.Key).ToList();
                var year = created.Year;
                //await _svc.UpdateQtys(itemids, year);

                return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
            }
            catch (Exception exx)
            {

                return BadRequest(exx);
            }
        }
        [HttpGet("{sdate}/{edate}")]
        public async Task<IActionResult> GetAll(DateTime sdate, DateTime edate)
        {
            // Use repository Query() if available; otherwise GetAllAsync and include details via DB context.
            var list = await _svc.GetDateWiseAsync(sdate, edate, CompanyId);




            return Ok(list);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var pm = await _svc.GetByIdAsync(id, CompanyId);
            if (pm == null) return NotFound();
            return Ok(pm);
        }
		[HttpPut("{id:int}")]
		public async Task<IActionResult> Update(int id, [FromBody] PurchaseMasterDto model)
		{

			try
			{
				if (model == null || id != model.Id)
					return BadRequest();

				var existing = await _context.PurchaseMasters
					.Include(p => p.Details)
					.FirstOrDefaultAsync(p => p.Id == id);

				if (existing == null)
					return NotFound();

				// update master
				existing.Date = model.Date;
				existing.SubTotal = model.SubTotal;
				existing.Discount = model.Discount;
				existing.TaxAmount = model.TaxAmount;
				existing.Total = model.SubTotal + model.TaxAmount - model.Discount;
				existing.VendorID = model.VendorID;
				existing.BranchId = model.BranchId;
				existing.UserId = CurrentUser.Id;
				existing.Active = 1;

				// ---------- SYNC DETAILS ----------

				// 1. Remove deleted details
				var toRemove = existing.Details
					.Where(ed => !model.Details.Any(md => md.Id == ed.Id))
					.ToList();

				foreach (var rem in toRemove)
					_context.PurchaseDetails.Remove(rem);

				// 2. Update or Add
				foreach (var d in model.Details)
				{
					if (d.Id > 0)
					{
						var existDetail = existing.Details.FirstOrDefault(x => x.Id == d.Id);
						if (existDetail != null)
						{
							existDetail.ItemId = d.ItemId;
							existDetail.Rate = d.Rate;
							existDetail.Qty = d.Qty;
							existDetail.Discount = d.Discount;
							existDetail.TaxPercentage = d.TaxPercentage;
							existDetail.TaxAmount = d.TaxAmount;
						}
					}
					else
					{
						existing.Details.Add(new PurchaseDetail
						{
							ItemId = d.ItemId,
							Rate = d.Rate,
							Qty = d.Qty,
							Discount = d.Discount,
							TaxPercentage = d.TaxPercentage,
							TaxAmount = d.TaxAmount
						});
					}
				}

				await _context.SaveChangesAsync();

				var ledgerService = new VendorLedgerService(_context);
				await ledgerService.UpdateLedgerAsync(existing);
			}
			catch (Exception exx)
			{
			
			}
			return NoContent();
		}


		// DELETE api/sales/{id}
		[HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _context.PurchaseMasters.FirstOrDefaultAsync(b => b.Id == id);
            if (existing == null) return NotFound();
            existing.Active = 0;
            _context.PurchaseMasters.Update(existing);

            await _context.SaveChangesAsync();
            var ledgerService = new VendorLedgerService(_context);
            await ledgerService.ReverseLedgerAsync(existing);
            return NoContent();
        }
    }

}

