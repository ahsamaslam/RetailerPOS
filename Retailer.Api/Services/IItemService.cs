using Retailer.POS.Api.DTOs;
namespace Retailer.POS.Api.Services;
public interface IItemService
{
    Task<IEnumerable<ItemDto>> GetAllAsync(Guid CompanyId);
    Task<IEnumerable<ItemDto>> GetStockItemsAsync(int categoryId = 0, int groupId = 0);
    Task<ItemDto?> GetByIdAsync(int id);
    Task<ItemDto> CreateAsync(CreateItemDto dto,Guid CompanyId);
    Task UpdateAsync(int id, CreateItemDto dto);
    Task DeleteAsync(int id);
    Task<IEnumerable<ItemDto>> SearchAsync(Guid companyId, string? term, int take = 20);

}
