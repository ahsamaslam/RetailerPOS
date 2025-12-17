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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private   string serverPath="";
        public CompaniesController(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            var request = _httpContextAccessor.HttpContext?.Request;
            serverPath =
            $"{request?.Scheme}://{request?.Host}{request?.PathBase}";
        }

        [Authorize]
        [HttpGet("Scenerio/{companyID:guid?}")]
        public async Task<IActionResult> GetScenarioCompany(Guid companyID ) 
        {
            // Extract CompanyId from token
            var companiesScnerio = await _db.CompanyScenario
                .Include(cs => cs.ScenarioMaster)
                .Include(cs => cs.Company)
                .Where(cs => cs.CompanyId == companyID)
                .ToListAsync(); 
            return Ok(companiesScnerio);
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
                IsActive = dto.IsActive ,
                CNIC = dto.CNIC,
                NTN  = dto.NTN,
                fbrActive = dto.fbrActive,
                pralToken= dto.pralToken,
                fbrToken = dto.fbrToken,
                 edVal = dto.edVal,
                 gstVal  = dto.gstVal,
                 fedVal= dto.fedVal,
                 isGst = dto.isGst,
                 isEd = dto.isEd,
                 isFed = dto.isFed,
                 
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
            company.logoPath = string.IsNullOrEmpty(company.logoPath) ? "" : serverPath + company.logoPath;
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
                 fbrToken = c.fbrToken  ,
                 pralToken  =c.pralToken,
                  invoiceCounter = c.invoiceCounter ,
                  invoicePerPage = c.invoicePerPage,
                  Province = c.Province,
                  CNIC= c.CNIC,NTN = c.NTN,
                logoPath= c.logoPath,
                isFed = c.isFed,
                isEd= c.isEd , 
                isGst = c.isGst,
                gstVal= c.gstVal,
                fedVal = c.fedVal,
                edVal = c.edVal,
              //  CompanyType = c.CompanyType

            };
    }
}
