using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Retailer.Api.DTOs;
using Retailer.Api.Infrastructure;
using Retailer.Api.Services;
using System.Security.Claims;

namespace Retailer.Api.Controllers
{
    [ApiController]
    [Route("api/menus")]
    [Authorize]
    public class MenusController : ControllerBase
    {
        private readonly IMenuService _menuService;
        public MenusController(IMenuService menuService) => _menuService = menuService;

        /// <summary>
        /// Admin: list all menus (including submenus)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll() => Ok(await _menuService.GetAllMenusAsync());

        /// <summary>
        /// Admin: get menu by id
        /// </summary>
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get(int id)
        {
            var dto = await _menuService.GetMenuByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        /// <summary>
        /// Admin: create top-level menu
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] MenuDto dto)
        {
            if (dto == null) return BadRequest();
            var created = await _menuService.CreateMenuAsync(dto);
            if (created == null) return BadRequest("Failed to create menu");
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        /// <summary>
        /// Admin: update top-level menu
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] MenuDto dto)
        {
            if (dto == null) return BadRequest();
            var ok = await _menuService.UpdateMenuAsync(id, dto);
            if (!ok) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Admin: delete top-level menu
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _menuService.DeleteMenuAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        // ----------------- Submenu endpoints -----------------

        /// <summary>
        /// Admin: create a submenu under a menu
        /// POST api/menus/{menuId}/submenus
        /// </summary>
        [HttpPost("{menuId:int}/submenus")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSubMenu(int menuId, [FromBody] SubMenuDto dto)
        {
            if (dto == null) return BadRequest();
            // enforce parent id
            if (dto.MenuId != 0 && dto.MenuId != menuId) return BadRequest("MenuId mismatch");

            dto.MenuId = menuId;
            var created = await _menuService.CreateSubMenuAsync(menuId, dto);
            if (created == null) return BadRequest("Failed to create submenu");

            // return Created at the parent menu resource (could also return the submenu location)
            return CreatedAtAction(nameof(Get), new { id = menuId }, created);
        }

        /// <summary>
        /// Admin: delete a submenu
        /// DELETE api/menus/{menuId}/submenus/{subMenuId}
        /// </summary>
        [HttpDelete("{menuId:int}/submenus/{subMenuId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSubMenu(int menuId, int subMenuId)
        {
            var ok = await _menuService.DeleteSubMenuAsync(menuId, subMenuId);
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
