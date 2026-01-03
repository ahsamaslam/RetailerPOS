using Retailer.POS.Api.DTOs;
namespace Retailer.POS.Api.Services;
public interface IPurchaseService
{
    Task<PurchaseMasterDto> CreatePurchaseAsync(CreatePurchaseDto dto, Guid CompanyId,Guid UserId); 
    Task<PurchaseMasterDto?> GetByIdAsync(int id, Guid companyID);
    Task<IEnumerable<PurchaseMasterDto?>> GetDateWiseAsync(DateTime sdate , DateTime edate, Guid CompanyId);
    Task<IEnumerable<PurchaseMasterDto?>> GetAll(Guid CompanyId);
}
