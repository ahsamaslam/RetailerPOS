using Retailer.Api.DtoReport;
using Retailer.Api.DTOs;
using Retailer.POS.Api.DTOs;
using Retailer.POS.Api.Entities;

namespace Retailer.POS.Api.Services;

public interface ISalesService
{
    Task<List<SalesMasterDto?>> GetDateWiseAsync(DateTime sdate, DateTime edate, Guid CompanyId);
    Task<List<SalesMasterDto?>> GetCustomerWiseAsync(int vendorID, DateTime sdate, DateTime edate, Guid CompanyId);
    Task<List<ItemSalesReportDtoR?>> GetItemWiseAsync(int itemID, DateTime sdate, DateTime edate, Guid CompanyId);
    Task<SalesMasterDto?> GetAsync(int id, Guid companyId, LoginDto user);
    Task<SalesMasterDto> CreateAsync(SalesMasterDto model, Guid companyId, LoginDto user);
    Task UpdateAsync(int id, SalesMaster model, Guid companyId, LoginDto user);
    Task DeleteAsync(int id);
}
