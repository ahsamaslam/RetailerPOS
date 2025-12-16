using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Retailer.Api.Entities;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Repositories;

namespace Retailer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BanksController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        public BanksController(IUnitOfWork uow) => _uow = uow;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _uow.Banks.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var entity = await _uow.Banks.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Banks model)
        {
            if (string.IsNullOrWhiteSpace(model.AccountNumber))
                return BadRequest(new { message = "Account Name is required." });

            // 🔍 Check if name already exists (case-insensitive)
            var exists = await _uow.Banks.GetAllAsync()
                .ContinueWith(t => t.Result
                .Any(c => c.AccountNumber.Equals(model.AccountNumber, StringComparison.OrdinalIgnoreCase)));
            
            if (exists)
                return Conflict(new { message = "City name already exists." });

            await _uow.Banks.AddAsync(model);
            await _uow.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int Id, [FromBody] Banks model)
        {
            var existing = await _uow.Banks.GetByIdAsync(Id);
            if (existing == null) return NotFound();
            existing.AccountNumber = model.AccountNumber;
            existing.AccountName = model.AccountName;
            existing.BrnchName = model.BrnchName;
            existing.BranchCode = model.BranchCode;
            existing.Mobile = model.Mobile;
            existing.Phone = model.Phone;
            existing.Address = model.Address;
            existing.CityId = model.CityId;
            existing.openDate  = model.openDate;
            existing.openingBalance  = model.openingBalance;
            _uow.Banks.Update(existing);
            await _uow.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _uow.Banks.GetByIdAsync(id);
            if (existing == null) return NotFound();
            _uow.Banks.Remove(existing);
            await _uow.SaveChangesAsync();
            return NoContent();
        }
    }
}
