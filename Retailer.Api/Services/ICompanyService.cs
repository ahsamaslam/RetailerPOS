using Retailer.Api.DTOs;

namespace Retailer.Api.Services
{
    public interface ICompanyService
    {
        Task<IEnumerable<CompanyDto>> GetAllCompanyAsync();
        Task<CompanyDto?> GetCompanyByIdAsync(Guid id);
        Task<CompanyDto?> CreateCompanyAsync(CompanyDto dto);
        Task<bool> UpdateCompanyAsync(Guid id, CompanyDto dto);
        Task<bool> DeleteCompanyAsync(int id); 
        Task<IEnumerable<CompanyDto>> GetCompanysForUserAsync(string userId);
    }
}
