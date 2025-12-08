using Retailer.POS.Api.DTOs;
namespace Retailer.POS.Api.Services;
public interface IPurchaseService
{
    Task<PurchaseMasterDto> CreatePurchaseAsync(CreatePurchaseDto dto);
    Task<bool> UpdateQtys(List<int> productIDs , int year);
    Task<PurchaseMasterDto?> GetByIdAsync(int id);
    Task<IEnumerable<PurchaseMasterDto?>> GetDateWiseAsync(DateTime sdate , DateTime edate);
    Task<IEnumerable<PurchaseMasterDto?>> GetAll();
}
