using AuthModule.API.Data;
using AuthModule.API.Dtos;
using AuthModule.API.Models;
using AuthModule.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AuthModule.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompaniesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private string serverPath = "";
        public CompaniesController(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            var request = _httpContextAccessor.HttpContext?.Request;
            serverPath =
            $"{request?.Scheme}://{request?.Host}{request?.PathBase}";
        }
        private Guid CompanyId => HttpContext.GetCompanyId();

        [Authorize(Roles = "superadmin")]
        [HttpGet("search")]
        public async Task<IActionResult> SearchCompanies(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return Ok(Array.Empty<object>());

            var companies = await _db.Companies
                .Where(c => c.Name.Contains(q))
                .OrderBy(c => c.Name)
                .Select(c => new
                {
                    c.Id,
                    c.Name
                })
                .Take(10)
                .ToListAsync();

            return Ok(companies);
        }

        [Authorize(Roles = "superadmin")]
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
                IsActive = dto.IsActive,
                CNIC = dto.CNIC,
                NTN = dto.NTN,
                fbrActive = dto.fbrActive,
                pralToken = dto.pralToken,
                fbrToken = dto.fbrToken,
                edVal = dto.edVal,
                gstVal = dto.gstVal,
                fedVal = dto.fedVal,
                isGst = dto.isGst,
                isEd = dto.isEd,
                isFed = dto.isFed,

            };
            await _db.Companies.AddAsync(company);
            await _db.SaveChangesAsync();
            var resp = ToResponseDto(company);
            return CreatedAtAction(nameof(Get), new { id = company.Id }, resp);
        }

        [Authorize(Roles = "superadmin")]
        // GET: api/companies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompanyResponseDto>>> GetAll()
        {
            var companies = await _db.Companies
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => ToResponseDto(c))
                .ToListAsync();
            return Ok(companies);
        }

        [Authorize(Roles = "superadmin")]
        // GET: api/companies/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CompanyResponseDto>> Get(Guid id)
        {
            var company = await _db.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            company.logoPath = string.IsNullOrEmpty(company.logoPath) ? "" : serverPath + company.logoPath;
            if (company == null) return NotFound();
            return Ok(ToResponseDto(company));
        }

        [Authorize(Roles = "superadmin")]
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
            company.fbrActive = dto.fbrActive;
            company.pralToken = dto.pralToken;
            company.fbrToken = dto.fbrToken;
            company.STRN = dto.STRN;
            company.NTN = dto.NTN;
            company.Province = dto.Province;
            company.logoPath = dto.logoPath;
            company.isEd = dto.isEd;
            company.isGst = dto.isGst;
            company.isFed = dto.isFed;
            company.edVal = dto.edVal;
            company.gstVal = dto.gstVal;
            company.fedVal = dto.fedVal;
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
                IsActive = c.IsActive,
                fbrActive = c.fbrActive,
                fbrToken = c.fbrToken,
                pralToken = c.pralToken,
                invoiceCounter = c.invoiceCounter,
                invoicePerPage = c.invoicePerPage,
                Province = c.Province,
                CNIC = c.CNIC,
                NTN = c.NTN,
                logoPath = c.logoPath,
                isFed = c.isFed,
                isEd = c.isEd,
                isGst = c.isGst,
                gstVal = c.gstVal,
                fedVal = c.fedVal,
                edVal = c.edVal,
                //  CompanyType = c.CompanyType

            };
        [Authorize]
        [HttpGet("User")]
        public async Task<IActionResult> GetUserCompany()

        {
            // Extract CompanyId from token
            var company = await _db.Companies.FirstOrDefaultAsync(c => c.Id == CompanyId);

            // Fetch company from database
       
            if (company == null)
                return NotFound("Company not found.");

            return Ok(ToResponseDto(company));
        }
    }
}
