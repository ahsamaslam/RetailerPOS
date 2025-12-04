using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Retailer.Api.DTOs;
using Retailer.Api.Entities;
using Retailer.POS.Api.Repositories;

namespace Retailer.Api.Controllers
{
    [ApiController]
    [Route("api/openingbalances")]
    public class OpeningBalancesController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<OpeningBalancesController> _logger;

        public OpeningBalancesController(IUnitOfWork uow, ILogger<OpeningBalancesController> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        // GET: api/openingbalances
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _uow.OpeningBalances
                                 .Query()
                                 .OrderByDescending(x => x.Year)
                                 .ThenBy(x => x.Product)
                                 .Select(x => new OpeningBalanceViewModel
                                 {
                                     Id = x.Id,
                                     Year = x.Year,
                                     Product = x.Product,
                                     OpeningQuantity = x.OpeningQuantity,
                                     CreatedAt = x.CreatedAt
                                 }).ToListAsync();

            return Ok(list);
        }

        // GET: api/openingbalances/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var entity = await _uow.OpeningBalances.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return Ok(new OpeningBalanceViewModel
            {
                Id = entity.Id,
                Year = entity.Year,
                Product = entity.Product,
                OpeningQuantity = entity.OpeningQuantity,
                CreatedAt = entity.CreatedAt
            });
        }

        // GET: api/openingbalances/year/{year}
        [HttpGet("year/{year:int}")]
        public async Task<IActionResult> GetByYear(int year)
        {
            var list = await _uow.OpeningBalances
                                 .Query()
                                 .Where(x => x.Year == year)
                                 .OrderBy(x => x.Product)
                                 .Select(x => new OpeningBalanceViewModel
                                 {
                                     Id = x.Id,
                                     Year = x.Year,
                                     Product = x.Product,
                                     OpeningQuantity = x.OpeningQuantity,
                                     CreatedAt = x.CreatedAt
                                 }).ToListAsync();

            return Ok(list);
        }

        // POST: api/openingbalances
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOpeningBalanceDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // uniqueness check (Year + Product)
            var exists = await _uow.OpeningBalances
                                   .Query()
                                   .AnyAsync(x => x.Year == dto.Year && x.Product == dto.Product);
            if (exists)
                return Conflict("Opening balance for this Year and Product already exists.");

            var entity = new OpeningBalance
            {
                Year = dto.Year,
                Product = dto.Product,
                OpeningQuantity = dto.OpeningQuantity,
                CreatedAt = DateTime.UtcNow
            };

            await _uow.OpeningBalances.AddAsync(entity);
            await _uow.SaveChangesAsync();

            var vm = new OpeningBalanceViewModel
            {
                Id = entity.Id,
                Year = entity.Year,
                Product = entity.Product,
                OpeningQuantity = entity.OpeningQuantity,
                CreatedAt = entity.CreatedAt
            };

            return CreatedAtAction(nameof(Get), new { id = entity.Id }, vm);
        }

        // PUT: api/openingbalances/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateOpeningBalanceDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != dto.Id) return BadRequest("Id mismatch.");

            var entity = await _uow.OpeningBalances.GetByIdAsync(id);
            if (entity == null) return NotFound();

            // uniqueness check for Year+Product excluding current record
            var exists = await _uow.OpeningBalances
                                   .Query()
                                   .AnyAsync(x => x.Id != id && x.Year == dto.Year && x.Product == dto.Product);
            if (exists)
                return Conflict("Another opening balance exists for this Year and Product.");

            entity.Year = dto.Year;
            entity.Product = dto.Product;
            entity.OpeningQuantity = dto.OpeningQuantity;

            _uow.OpeningBalances.Update(entity);
            try
            {
                await _uow.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error while updating OpeningBalance {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Concurrency error updating record.");
            }
        }

        // DELETE: api/openingbalances/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _uow.OpeningBalances.GetByIdAsync(id);
            if (entity == null) return NotFound();

            _uow.OpeningBalances.Remove(entity);
            await _uow.SaveChangesAsync();

            return NoContent();
        }
    }
}
