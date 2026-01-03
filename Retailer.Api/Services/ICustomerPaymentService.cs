using Retailer.Api.DTOs;
using Retailer.POS.Api.DTOs;
namespace Retailer.POS.Api.Services;
public interface ICustomerPaymentService
{
    Task<IEnumerable<CustomerPaymentDto>> GetAllAsync(Guid CompanyId); 
    Task<CustomerPaymentDto?> GetByIdAsync(int id);
    Task<IEnumerable<CustomerPaymentDto>?> GetByDateWiseAsync(DateTime sdate, DateTime edate, Guid CompanyId);
    Task<CustomerPaymentDto> CreateAsync(CustomerPaymentDto dto,Guid CompanyId);
    Task UpdateAsync(int id, CustomerPaymentDto dto);
    Task DeleteAsync(int id);
}
