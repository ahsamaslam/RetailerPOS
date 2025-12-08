using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Repositories; // your IUnitOfWork namespace
using Retailer.POS.Api.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Retailer.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Retailer.Api.Services; // optional DTO namespace if you have

namespace Retailer.POS.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IFbrClient _fbrClient;
        private readonly ICompanyService _companyService;
        public SalesController(IUnitOfWork uow, IFbrClient fbrClient, ICompanyService companyService)
        {
            _uow = uow;
            _fbrClient = fbrClient;
            _companyService = companyService;
        }

          
        [HttpGet]
        [HttpGet("GetAllDateWise/{sdate}/{edate}")]
        public async Task<IActionResult> GetAllDateWise(DateTime sdate, DateTime edate)
        {
            // Use repository Query() if available; otherwise GetAllAsync and include details via DB context.
            var list = await _uow.SalesMasters

                .Query()
                .Where(r => r.Date.Date >= sdate.Date.Date && r.Date.Date <= edate.Date)
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
            try
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
                var itemids = model.Details.GroupBy(x => x.ItemCode).Select(x => x.Key).ToList();
                var year = model.Year;
                var companyIdClaim = User.FindFirst("companyId")?.Value;
                if (!string.IsNullOrEmpty(companyIdClaim) && Guid.TryParse(companyIdClaim, out var companyId))
                {
                    var company = await _companyService.GetCompanyByIdAsync(companyIdClaim);
                    if (company?.fbrActive == true)
                    {
                        // send invoice to FBR
                        try
                        {
                            Customer customer = _uow.Customers.Query().Where(r => r.Id == model.CustomerCode).First(); ;
                            var fbrResult = await _fbrClient.SendInvoiceAsync(company, model, customer);
                            var created = model;
                            if (!fbrResult.Success)
                            {
                                // _logger.LogWarning("FBR invoice send failed for sale {SaleId}: {Message}", model.Id, fbrResult.Message);
                                // decide whether to mark sale as 'FBRFailed' or queue for retry
                                //    await _salesService.MarkSaleFbrStatusAsync(created.Id, false, fbrResult.Message);
                            }
                            else
                            {
                                //_logger.LogInformation("FBR invoice accepted for sale {SaleId} externalId={ExternalId}", created.Id, fbrResult.ExternalId);
                                // await _salesService.MarkSaleFbrStatusAsync(created.Id, true, fbrResult.ExternalId);
                            }
                        }
                        catch (Exception ex)
                        {
                            //_logger.LogError(ex, "Exception while sending invoice to FBR for sale {SaleId}", model.Id) ;
                            //     await _salesService.MarkSaleFbrStatusAsync(model.Id, false, ex.Message);
                        }
                    }
                }
                else
                {
                    //_logger.LogDebug("No companyId claim present; skipping FBR send");
                }
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
