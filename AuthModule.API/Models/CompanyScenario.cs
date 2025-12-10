using System.ComponentModel.DataAnnotations;

namespace AuthModule.API.Models
{
    public class CompanyScenario
    {
        [Key]
        public Guid Guid { get; set; }  
        public Guid CompanyId { get; set; }
        public Company Company { get; set; }
        public string ScenarioMasterId { get; set; }    
        public ScenarioMaster ScenarioMaster { get; set; }
    }
}
