using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Retailer.Api.DataSet;
using Retailer.Api.DTOs;
using Retailer.Api.Infrastructure;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Services; // your IUnitOfWork namespace

namespace Retailer.POS.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ISalesService _salesService;

        public SalesController(ISalesService salesService)
        {
            _salesService = salesService;
        }

        private Guid CompanyId => HttpContext.GetCompanyId();
        private LoginDto CurrentUser => HttpContext.GetUserId();

        [HttpGet("GetAllDateWise/{sdate}/{edate}")]
        public async Task<IActionResult> GetAllDateWise(DateTime sdate, DateTime edate)
            => Ok(await _salesService.GetDateWiseAsync(sdate, edate, CompanyId));

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
            => Ok(await _salesService.GetAsync(id, CompanyId, CurrentUser));

        [HttpGet("CustomerWise/{customerID}/{sdate}/{edate}")]
        public async Task<IActionResult> GetAll(int customerID, DateTime sdate, DateTime edate)
        {
            // Use repository Query() if available; otherwise GetAllAsync and include details via DB context.
            var list = await _salesService.GetCustomerWiseAsync(customerID, sdate, edate, CompanyId);

            return Ok(list);
        }
        [HttpGet("ItemWise/{itemID}/{sdate}/{edate}")]
        public async Task<IActionResult> ItemWiseGetAll(int itemID, DateTime sdate, DateTime edate)
        {
            // Use repository Query() if available; otherwise GetAllAsync and include details via DB context.
            var list = await _salesService.GetItemWiseAsync(itemID, sdate, edate, CompanyId);

            return Ok(list);

        }


            [HttpPost]
        public async Task<IActionResult> Create([FromBody] SalesMasterDto model)
            => Ok(await _salesService.CreateAsync(model, CompanyId, CurrentUser));

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] SalesMaster model)
        {
            await _salesService.UpdateAsync(id, model, CompanyId, CurrentUser);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _salesService.DeleteAsync(id);
            return NoContent();
        }
    }
}
