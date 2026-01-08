using Retailer.Api.DtoReport;
using Retailer.POS.Api.DTOs;
namespace Retailer.POS.Api.Services;
public interface IPurchaseReturnService
{
    Task<PurchaseReturnMasterDto> CreatePurchaseAsync(CreatePurchaseReturnDto dto, Guid CompanyId,Guid UserId);
    Task<PurchaseReturnMasterDto?> GetByIdAsync(int id, Guid companyID);
    Task<IEnumerable<PurchaseReturnMasterDto?>> GetDateWiseAsync(DateTime sdate , DateTime edate, Guid CompanyId);
    Task<IEnumerable<PurchaseReturnMasterDto?>> GetVendorWiseAsync(int vendorID , DateTime sdate , DateTime edate, Guid CompanyId);
    Task<IEnumerable<ItemPurchaseReturnReportDtoR?>> GetItemWiseAsync(int itemID , DateTime sdate , DateTime edate, Guid CompanyId);
    Task<IEnumerable<PurchaseReturnMasterDto?>> GetAll(Guid CompanyId);
}
