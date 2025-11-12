using Microsoft.AspNetCore.Mvc;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Repositories; // namespace where IUnitOfWork lives

namespace Retailer.POS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BranchController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public BranchController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _uow.Branches.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var entity = await _uow.Branches.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Branch model)
        {
            if (model == null) return BadRequest();
            await _uow.Branches.AddAsync(model);
            await _uow.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Branch model)
        {
            if (model == null || id != model.Id) return BadRequest();

            var existing = await _uow.Branches.GetByIdAsync(id);
            if (existing == null) return NotFound();

            existing.Name = model.Name;
            existing.Address = model.Address;
            existing.ContactPerson = model.ContactPerson;
            existing.MobileNo = model.MobileNo;
            existing.BillHeading1 = model.BillHeading1;
            existing.BillHeading2 = model.BillHeading2;
            existing.BillMobileNo = model.BillMobileNo;

            _uow.Branches.Update(existing);
            await _uow.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _uow.Branches.GetByIdAsync(id);
            if (existing == null) return NotFound();

            _uow.Branches.Remove(existing);
            await _uow.SaveChangesAsync();
            return NoContent();
        }
    }
}
