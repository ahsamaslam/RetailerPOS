using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Retailer.Api.DTOs;
using Retailer.Api.Entities;
using Retailer.POS.Api.Data;
using System.Net.Http;

namespace Retailer.Api.Services
{
    public class ScenarioMasterService : IScenarioMaster
    {
        private readonly HttpClient _httpClient;
        private readonly RetailerDbContext _db;
        private readonly IMemoryCache _cache;

        public ScenarioMasterService(
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
        public async Task<IEnumerable<ScenarioMasterDto>> GetAllScenarioAsync()
        {
            var response = await _httpClient.GetAsync($"api/ScenarioMaster");
            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException("Failed to fetch ScenarioMaster from AuthModule");

            var companyList = await response.Content.ReadFromJsonAsync<IEnumerable<ScenarioMasterDto>>();
            return companyList;
        }

        public async Task<ScenarioMasterDto?> GetScenarioByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    throw new ArgumentNullException(nameof(id));

                var response = await _httpClient.GetAsync($"api/ScenarioMaster/" + id);
                if (!response.IsSuccessStatusCode)
                    throw new InvalidOperationException("Failed to fetch Company from AuthModule");

                var ScenarioMasterDto = await response.Content.ReadFromJsonAsync<ScenarioMasterDto>();

                return ScenarioMasterDto;
            }
            catch (Exception exx)
            { 
            throw new InvalidOperationException("Failed to fetch Company from AuthModule",exx);
            }

        }
     

        public async Task<ScenarioMasterDto?> CreateScenarioAsync(ScenarioMasterDto scenario)
        {
            if (scenario is null)
                throw new ArgumentNullException(nameof(scenario));

            var response = await _httpClient.PostAsJsonAsync("api/ScenarioMaster", scenario);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException("Failed to create Scenario in AuthModule");

            var createdscenario = await response.Content.ReadFromJsonAsync<ScenarioMasterDto>();

            return createdscenario;
        }

        public async Task<bool> UpdateScenarioAsync(string id, ScenarioMasterDto dto)
        {
            
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            var response = await _httpClient.PutAsJsonAsync($"api/ScenarioMaster/{id}", dto);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException("Failed to update Scenario in AuthModule");

            return true;
        }
        public async Task<bool> DeleteScenarioAsync(string id)
        {
           
            var response = await _httpClient.DeleteAsync($"api/ScenarioMaster/{id}");

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException("Failed to delete Company in AuthModule");

            return true;
        }
         

    }
}
