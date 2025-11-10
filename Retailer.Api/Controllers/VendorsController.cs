using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Repositories;

namespace Retailer.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class VendorsController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        public VendorsController(IUnitOfWork uow) => _uow = uow;

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _uow.Vendors.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var v = await _uow.Vendors.GetByIdAsync(id);
            if (v == null) return NotFound();
            return Ok(v);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Vendor model)
        {
            await _uow.Vendors.AddAsync(model);
            await _uow.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
        }
    }
}