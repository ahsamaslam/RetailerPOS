using Retailer.Api.DTOs;

namespace Retailer.Api.Services
{
    public interface IScenarioMaster
    {
        Task<IEnumerable<ScenarioMasterDto>> GetAllScenarioAsync();
        Task<ScenarioMasterDto?> GetScenarioByIdAsync(string id);
        Task<ScenarioMasterDto?> CreateScenarioAsync(ScenarioMasterDto dto);
        Task<bool> UpdateScenarioAsync(string id, ScenarioMasterDto dto);
        Task<bool> DeleteScenarioAsync(string id);  
    }
}
