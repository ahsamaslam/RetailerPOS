using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Retailer.Api.DTOs;
using Retailer.Api.Services;
using System.Security.Claims;

namespace Retailer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ScenarioMasterController : ControllerBase
    {
        private readonly IScenarioMaster _scenarioMaster;
        public ScenarioMasterController(IScenarioMaster scenarioMaster) => _scenarioMaster = scenarioMaster;
 
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll() => Ok(await _scenarioMaster.GetAllScenarioAsync());

        /// <summary>
        /// Admin: get company by id
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(string id)
        {
            var dto = await _scenarioMaster.GetScenarioByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }
       
        /// <summary>
        /// Admin: create top-level company
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] ScenarioMasterDto dto)
        {
            if (dto == null) return BadRequest();
            var created = await _scenarioMaster.CreateScenarioAsync(dto);
            if (created == null) return BadRequest("Failed to create scernio");
            return CreatedAtAction(nameof(Get), new { id = created.ScenarioId }, created);
        }

        /// <summary>
        /// Admin: update top-level company
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(string id, [FromBody] ScenarioMasterDto dto)
        {
            if (dto == null) return BadRequest();
            var ok = await _scenarioMaster.UpdateScenarioAsync(id, dto);
            if (!ok) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Admin: delete top-level company
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var ok = await _scenarioMaster.DeleteScenarioAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        } 
           
    }

}
