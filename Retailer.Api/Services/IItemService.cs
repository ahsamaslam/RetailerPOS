using Retailer.POS.Api.DTOs;
namespace Retailer.POS.Api.Services;
public interface IItemService
{
    Task<IEnumerable<ItemDto>> GetAllAsync();
    Task<IEnumerable<ItemDto>> GetStockItemsAsync(int categoryId = 0, int groupId = 0);
    Task<ItemDto?> GetByIdAsync(int id);
    Task<ItemDto> CreateAsync(CreateItemDto dto);
    Task UpdateAsync(int id, CreateItemDto dto);
    Task DeleteAsync(int id);
}
