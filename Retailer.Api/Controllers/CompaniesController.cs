using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Retailer.Api.DTOs;
using Retailer.Api.Services;
using System.Security.Claims;

namespace Retailer.Api.Controllers
{
    [ApiController]
    [Route("api/Companies")]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        public CompaniesController(ICompanyService companyService) => _companyService = companyService;
 
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll() => Ok(await _companyService.GetAllCompanyAsync());

        /// <summary>
        /// Admin: get company by id
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(Guid id)
        {
            var dto = await _companyService.GetCompanyByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }
        [Authorize]
        [HttpGet("User")]
        public async Task<IActionResult> GetUserCompany()
        
        {
            // Extract CompanyId from token
            var companyIdString = User.FindFirst("companyId")?.Value;

            if (string.IsNullOrEmpty(companyIdString))
                return Unauthorized("companyId claim missing.");

            // Fetch company from database
            var company = await _companyService.GetCompanyByIdAsync(Guid.Parse(companyIdString));

            if (company == null)
                return NotFound("Company not found.");

            return Ok(company);
        }
        /// <summary>
        /// Admin: create top-level company
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CompanyDto dto)
        {
            if (dto == null) return BadRequest();
            var created = await _companyService.CreateCompanyAsync(dto);
            if (created == null) return BadRequest("Failed to create company");
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        /// <summary>
        /// Admin: update top-level company
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] CompanyDto dto)
        {
            if (dto == null) return BadRequest();
            var ok = await _companyService.UpdateCompanyAsync(id, dto);
            if (!ok) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Admin: delete top-level company
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _companyService.DeleteCompanyAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        } 
           
    }

}
