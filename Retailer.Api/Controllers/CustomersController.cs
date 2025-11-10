using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Repositories;

namespace Retailer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        public CustomersController(IUnitOfWork uow) => _uow = uow;

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _uow.Customers.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var c = await _uow.Customers.GetByIdAsync(id);
            if (c == null) return NotFound();
            return Ok(c);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Customer model)
        {
            await _uow.Customers.AddAsync(model);
            await _uow.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
        }
    }
}
