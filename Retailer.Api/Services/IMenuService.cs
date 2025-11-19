using Retailer.Api.DTOs;

namespace Retailer.Api.Services
{
    public interface IMenuService
    {
        Task<IEnumerable<MenuDto>> GetAllMenusAsync(); // admin
        Task<MenuDto?> GetMenuByIdAsync(int id);
        Task<MenuDto> CreateMenuAsync(MenuDto dto);
        Task<bool> UpdateMenuAsync(int id, MenuDto dto);
        Task<bool> DeleteMenuAsync(int id);

        // returns only menus the user is authorized to see
        Task<IEnumerable<MenuDto>> GetMenusForUserAsync(string userId);
    }
}
