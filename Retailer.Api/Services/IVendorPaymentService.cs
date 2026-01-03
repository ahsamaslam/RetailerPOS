using Retailer.Api.DTOs;
using Retailer.POS.Api.DTOs;
namespace Retailer.POS.Api.Services;
public interface IVendorPaymentService
{
    Task<IEnumerable<VendorPaymentDto>> GetAllAsync(Guid CompanyId); 
    Task<VendorPaymentDto?> GetByIdAsync(int id);
    Task<IEnumerable<VendorPaymentDto>?> GetByDateWiseAsync(DateTime sdate, DateTime edate, Guid CompanyId);
    Task<VendorPaymentDto> CreateAsync(VendorPaymentDto dto,Guid CompanyId);
    Task UpdateAsync(int id, VendorPaymentDto dto);
    Task DeleteAsync(int id);
}
