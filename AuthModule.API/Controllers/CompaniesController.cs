using AuthModule.API.Data;
using AuthModule.API.Dtos;
using AuthModule.API.Models;
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
    public class CompaniesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public CompaniesController(ApplicationDbContext db)
        {
            _db = db;
        }

        // POST: api/companies
        [HttpPost]
        public async Task<ActionResult<CompanyResponseDto>> Create([FromBody] CompanyCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var company = new Company
            {
                Id = Guid.NewGuid(),
                Name = dto.Name.Trim(),
                ShortName = dto.ShortName?.Trim(),
                Address = dto.Address,
                ContactEmail = dto.ContactEmail,
                ContactPhone = dto.ContactPhone,
                IsActive = dto.IsActive
            };

            await _db.Companies.AddAsync(company);
            await _db.SaveChangesAsync();

            var resp = ToResponseDto(company);
            return CreatedAtAction(nameof(GetById), new { id = company.Id }, resp);
        }

        // GET: api/companies
        [HttpGet]
        [AllowAnonymous] // allow public listing — change if needed
        public async Task<ActionResult<IEnumerable<CompanyResponseDto>>> GetAll()
        {
            var companies = await _db.Companies
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => ToResponseDto(c))
                .ToListAsync();

            return Ok(companies);
        }

        // GET: api/companies/{id}
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<CompanyResponseDto>> GetById(Guid id)
        {
            var company = await _db.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            if (company == null) return NotFound();

            return Ok(ToResponseDto(company));
        }

        // PUT: api/companies/{id}
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<CompanyResponseDto>> Update(Guid id, [FromBody] CompanyUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var company = await _db.Companies.FirstOrDefaultAsync(c => c.Id == id);
            if (company == null) return NotFound();

            // update fields
            company.Name = dto.Name.Trim();
            company.ShortName = dto.ShortName?.Trim();
            company.Address = dto.Address;
            company.ContactEmail = dto.ContactEmail;
            company.ContactPhone = dto.ContactPhone;
            company.IsActive = dto.IsActive;

            _db.Companies.Update(company);
            await _db.SaveChangesAsync();

            return Ok(ToResponseDto(company));
        }

        // OPTIONAL: delete a company (soft delete recommended in prod)
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var company = await _db.Companies.FirstOrDefaultAsync(c => c.Id == id);
            if (company == null) return NotFound();

            _db.Companies.Remove(company);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // simple mapper
        private static CompanyResponseDto ToResponseDto(Company c) =>
            new CompanyResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                ShortName = c.ShortName,
                Address = c.Address,
                ContactEmail = c.ContactEmail,
                ContactPhone = c.ContactPhone,
                IsActive = c.IsActive
            };
    }
}
