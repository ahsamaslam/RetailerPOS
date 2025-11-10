using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Repositories;

namespace Retailer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        public EmployeesController(IUnitOfWork uow) => _uow = uow;

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _uow.Employees.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var e = await _uow.Employees.GetByIdAsync(id);
            if (e == null) return NotFound();
            return Ok(e);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Employee model)
        {
            await _uow.Employees.AddAsync(model);
            await _uow.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
        }
    }
}
