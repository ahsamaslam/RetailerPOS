using AuthModule.API.Data;
using AuthModule.API.Dtos;
using AuthModule.API.Models;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthModule.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // Optional: restrict to Admins. Remove or change as needed.
    //[Authorize(Roles = "SuperAdmin")]
    public class ScenarioMasterController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public ScenarioMasterController(ApplicationDbContext db)
        {
            _db = db;
        }

        // POST: api/ScenarioMaster
        [HttpPost]
        public async Task<ActionResult<ScenarioMasterDto>> Create([FromBody] ScenarioMasterDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            bool nameExists = await _db.ScenarioMaster
        .AnyAsync(x => x.ScenarioName.ToLower() == dto.ScenarioName.ToLower()); 
            if (nameExists)
                return Conflict(new { message = "Scenario name already exists." });

            var scenarioMaster = new ScenarioMaster
            {
             ScenarioId = dto.ScenarioId,   
             ScenarioName = dto.ScenarioName,   
             SaleType = dto.SaleType,
             BuyerRegistrationType = dto.BuyerRegistrationType, 
             SroItemSerialNo = dto.SroItemSerialNo, 
             SroScheduleNo = dto.SroScheduleNo  
             
            };

            await _db.ScenarioMaster.AddAsync(scenarioMaster);
            await _db.SaveChangesAsync();

            var resp = ToResponseDto(scenarioMaster);
            return CreatedAtAction(nameof(GetById), new { id = scenarioMaster.ScenarioId }, resp);
        }

        // GET: api/ScenarioMaster
        [HttpGet]
        [AllowAnonymous] // allow public listing — change if needed
        public async Task<ActionResult<IEnumerable<ScenarioMasterDto>>> GetAll()
        {
            var ScenarioMaster = await _db.ScenarioMaster
                .AsNoTracking()
                .OrderBy(c => c.ScenarioId)
                .Select(c => ToResponseDto(c))
                .ToListAsync();

            return Ok(ScenarioMaster);
        }

        // GET: api/ScenarioMaster/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ScenarioMasterDto>> GetById(string id)
        {
            var ScenarioMaster = await _db.ScenarioMaster.AsNoTracking().FirstOrDefaultAsync(c => c.ScenarioId == id);
            if (ScenarioMaster == null) return NotFound();

            
            return Ok(ToResponseDto(ScenarioMaster));
        }

        // PUT: api/ScenarioMaster/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<ScenarioMasterDto>> Update(string id, [FromBody] ScenarioMasterDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var scenarioMaster = await _db.ScenarioMaster.FirstOrDefaultAsync(c => c.ScenarioId == id);
            if (scenarioMaster == null) return NotFound();

            // update fields
            scenarioMaster.ScenarioName = dto.ScenarioName.Trim();
            scenarioMaster.SroScheduleNo = dto.SroScheduleNo?.Trim();
            scenarioMaster.SaleType = dto.SaleType;
            scenarioMaster.BuyerRegistrationType = dto.BuyerRegistrationType;  
            _db.ScenarioMaster.Update(scenarioMaster);
            await _db.SaveChangesAsync(); 
            return Ok(ToResponseDto(scenarioMaster));
        } 
        // OPTIONAL: delete a scenarioMaster (soft delete recommended in prod)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var company = await _db.ScenarioMaster.FirstOrDefaultAsync(c => c.ScenarioId == id);
            if (company == null) return NotFound();

            _db.ScenarioMaster.Remove(company);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // simple mapper
        private static ScenarioMasterDto ToResponseDto(ScenarioMaster c) =>
            new ScenarioMasterDto
            {
                ScenarioId = c.ScenarioId,
                ScenarioName = c.ScenarioName,
                SaleType = c.SaleType,
                BuyerRegistrationType = c.BuyerRegistrationType,
                SroItemSerialNo = c.SroItemSerialNo,
                SroScheduleNo = c.SroScheduleNo


            };
    }
}
