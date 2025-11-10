using Retailer.POS.Api.DTOs;
namespace Retailer.POS.Api.Services;
public interface IItemService
{
    Task<IEnumerable<ItemDto>> GetAllAsync();
    Task<ItemDto?> GetByIdAsync(int id);
    Task<ItemDto> CreateAsync(CreateItemDto dto);
    Task UpdateAsync(int id, CreateItemDto dto);
    Task DeleteAsync(int id);
}
