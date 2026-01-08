using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Retailer.Api.DTOs;
using Retailer.Api.Infrastructure;
using Retailer.Api.Services; // optional DTO namespace if you have
using Retailer.POS.Api.Entities;

namespace Retailer.POS.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SalesReturnController : ControllerBase
    {
        private readonly ISalesReturnService _service;

        public SalesReturnController(ISalesReturnService service)
        {
            _service = service;
        }

        private Guid CompanyId => HttpContext.GetCompanyId();
        private LoginDto CurrentUser => HttpContext.GetUserId();

        [HttpGet("GetAllDateWise/{sdate}/{edate}")]
        public async Task<IActionResult> GetAllDateWise(DateTime sdate, DateTime edate)
            => Ok(await _service.GetDateWiseAsync(sdate, edate, CompanyId));

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
            => Ok(await _service.GetAsync(id, CompanyId, CurrentUser));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SalesReturnMaster model)
            => Ok(await _service.CreateAsync(model, CompanyId, CurrentUser));

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] SalesReturnMaster model)
        {
            await _service.UpdateAsync(id, model, CompanyId, CurrentUser);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
