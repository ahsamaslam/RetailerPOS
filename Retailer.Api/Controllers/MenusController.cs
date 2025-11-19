using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Retailer.Api.DTOs;
using Retailer.Api.Services;
using System.Security.Claims;

namespace Retailer.Api.Controllers
{
    [ApiController]
    [Route("api/menus")]
    public class MenusController : ControllerBase
    {
        private readonly IMenuService _menuService;
        public MenusController(IMenuService menuService) => _menuService = menuService;

        // Admin: list all (with permissions mapping)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll() => Ok(await _menuService.GetAllMenusAsync());

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get(int id)
        {
            var dto = await _menuService.GetMenuByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] MenuDto dto)
        {
            var created = await _menuService.CreateMenuAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] MenuDto dto)
        {
            var ok = await _menuService.UpdateMenuAsync(id, dto);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _menuService.DeleteMenuAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        // Public/user-facing: get menu tree for current user
        [HttpGet("me")]
        [Authorize] // any authenticated user
        public async Task<IActionResult> ForCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            var menus = await _menuService.GetMenusForUserAsync(userId);
            return Ok(menus);
        }
    }

}
