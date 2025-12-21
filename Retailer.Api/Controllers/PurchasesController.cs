using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Retailer.Api.DTOs;
using Retailer.Api.Infrastructure;
using Retailer.POS.Api.DTOs;
using Retailer.POS.Api.Services;

namespace Retailer.POS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PurchasesController : ControllerBase
    {
        private readonly IPurchaseService _svc;
        public PurchasesController(IPurchaseService svc) => _svc = svc;
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

                var created = await _svc.CreatePurchaseAsync(dto,CompanyId,CurrentUser.Id);
                var itemids = created.Details.GroupBy(x => x.ItemId).Select(x=>x.Key).ToList();
                var year = created.year;
                await _svc.UpdateQtys(itemids, year);     
                return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
            }
            catch (Exception exx)
            { 
            
            return BadRequest(exx); 
            }
        }
        [HttpGet("{sdate}/{edate}")]
        public async Task<IActionResult> GetAll(DateTime sdate , DateTime edate )
        {
            // Use repository Query() if available; otherwise GetAllAsync and include details via DB context.
            var list = await _svc.GetDateWiseAsync(sdate, edate,CompanyId);
               
               
                 

            return Ok(list);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var pm = await _svc.GetByIdAsync(id);
            if (pm == null) return NotFound();
            return Ok(pm);
        }
    }

}

