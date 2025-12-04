using Microsoft.AspNetCore.Mvc;
using Retailer.POS.Api.DTOs;
using Retailer.POS.Api.Services;

namespace Retailer.POS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchasesController : ControllerBase
    {
        private readonly IPurchaseService _svc;
        public PurchasesController(IPurchaseService svc) => _svc = svc;


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Use repository Query() if available; otherwise GetAllAsync and include details via DB context.
            var list = await _svc.GetAll();
                

            return Ok(list);
        }



        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePurchaseDto dto)
        {
            try
            {
                var created = await _svc.CreatePurchaseAsync(dto);
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
            var list = await _svc.GetDateWiseAsync(sdate, edate);
               
               
                 

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

