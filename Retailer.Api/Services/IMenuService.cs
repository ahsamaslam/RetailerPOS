using Retailer.Api.DTOs;

namespace Retailer.Api.Services
{
    public interface IMenuService
    {
        Task<IEnumerable<MenuDto>> GetAllMenusAsync();
        Task<MenuDto?> GetMenuByIdAsync(int id);
        Task<MenuDto?> CreateMenuAsync(MenuDto dto);
        Task<bool> UpdateMenuAsync(int id, MenuDto dto);
        Task<bool> DeleteMenuAsync(int id);

        // Submenus
        Task<SubMenuDto?> CreateSubMenuAsync(int menuId, SubMenuDto dto);
        Task<bool> DeleteSubMenuAsync(int menuId, int subMenuId);

        // returns only menus the user is authorized to see
        Task<IEnumerable<MenuDto>> GetMenusForUserAsync(string userId);
    }
}
