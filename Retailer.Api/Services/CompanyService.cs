using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Retailer.Api.DTOs;
using Retailer.Api.Entities;
using Retailer.POS.Api.Data;
using System.Net.Http;

namespace Retailer.Api.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly HttpClient _httpClient;
        private readonly RetailerDbContext _db;
        private readonly IMemoryCache _cache;

        public CompanyService(
            RetailerDbContext db,
            IMemoryCache cache,
            IHttpClientFactory httpClientFactory)
        {
            _db = db;
            _cache = cache;

            // get the named client
            _httpClient = httpClientFactory.CreateClient("AuthModule");
        }
        // Admin: full list
        public async Task<IEnumerable<CompanyDto>> GetAllCompanyAsync()
        {
            var response = await _httpClient.GetAsync($"api/Companies");
            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException("Failed to fetch Companies from AuthModule");

            var companyList = await response.Content.ReadFromJsonAsync<IEnumerable<CompanyDto>>();
            return companyList;
        }

        public async Task<CompanyDto?> GetCompanyByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    throw new ArgumentNullException(nameof(id));

                var response = await _httpClient.GetAsync($"api/Companies/" + id);
                if (!response.IsSuccessStatusCode)
                    throw new InvalidOperationException("Failed to fetch Company from AuthModule");

                var CompanyDto = await response.Content.ReadFromJsonAsync<CompanyDto>();

                return CompanyDto;
            }
            catch (Exception exx)
            { 
            throw new InvalidOperationException("Failed to fetch Company from AuthModule",exx);
            }

        }
        public async Task<CompanyDto?> GetCompanyByUseridAsync(string Userid)
        {
            if (string.IsNullOrWhiteSpace(Userid))
                throw new ArgumentNullException(nameof(Userid));

            var response = await _httpClient.GetAsync($"api/Companies/" + Userid.ToString());
            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException("Failed to fetch Company from AuthModule");

            var CompanyDto = await response.Content.ReadFromJsonAsync<CompanyDto>();

            return CompanyDto;

        }

        public async Task<CompanyDto?> CreateCompanyAsync(CompanyDto company)
        {
            if (company is null)
                throw new ArgumentNullException(nameof(company));

            var response = await _httpClient.PostAsJsonAsync("api/Companies", company);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException("Failed to create Company in AuthModule");

            var createdCompany = await response.Content.ReadFromJsonAsync<CompanyDto>();

            return createdCompany;
        }

        public async Task<bool> UpdateCompanyAsync(int id, CompanyDto dto)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than zero.");

            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            var response = await _httpClient.PutAsJsonAsync($"api/Companies/{id}", dto);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException("Failed to update Company in AuthModule");

            return true;
        }
        public async Task<bool> DeleteCompanyAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than zero.");

            var response = await _httpClient.DeleteAsync($"api/Companies/{id}");

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException("Failed to delete Company in AuthModule");

            return true;
        }

        public Task<IEnumerable<CompanyDto>> GetCompanysForUserAsync(string userId)
        {
            throw new NotImplementedException();
        }


        // ----- User-facing: return only Companys/subCompanys the user has permission for -----


    }
}
