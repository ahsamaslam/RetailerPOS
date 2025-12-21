using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Retailer.Api.Infrastructure;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Repositories;

namespace Retailer.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VendorsController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        public VendorsController(IUnitOfWork uow) => _uow = uow;
        private Guid CompanyId => HttpContext.GetCompanyId();

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _uow.Vendors.GetAllAsync(x => x.CompanyId == CompanyId));

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var v = await _uow.Vendors.GetAsync(b => b.Id == id);
            if (v == null) return NotFound();
            return Ok(v);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Vendor model)
        {
            model.CompanyId = CompanyId;
            await _uow.Vendors.AddAsync(model);
            await _uow.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
        }
    }
}