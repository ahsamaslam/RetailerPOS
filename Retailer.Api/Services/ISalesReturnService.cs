using Retailer.Api.DtoReport;
using Retailer.Api.DTOs;
using Retailer.POS.Api.Entities;

namespace Retailer.Api.Services
{
    public interface ISalesReturnService
    {
        Task<List<SalesReturnMasterDto?>> GetDateWiseAsync(DateTime sdate, DateTime edate, Guid CompanyId);
        Task<List<SalesReturnMasterDto?>> GetCustomerWiseAsync(int vendorID, DateTime sdate, DateTime edate, Guid CompanyId);
        Task<List<ItemSalesReturnReportDtoR?>> GetItemWiseAsync(int itemID, DateTime sdate, DateTime edate, Guid CompanyId);
        Task<SalesReturnMasterDto?> GetAsync(int id, Guid companyId, LoginDto user);
        Task<SalesReturnMaster> CreateAsync(SalesReturnMaster model, Guid companyId, LoginDto user);
        Task UpdateAsync(int id, SalesReturnMaster model, Guid companyId, LoginDto user);
        Task DeleteAsync(int id);
    }

}
