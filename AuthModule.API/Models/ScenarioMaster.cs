using System.ComponentModel.DataAnnotations;

namespace AuthModule.API.Models
{
    public class ScenarioMaster
    {
        [Key] 
        public string ScenarioId { get; set; }  
        public string ScenarioName { get; set; }  
        public string SaleType { get; set; }  
        public string SroScheduleNo { get; set; }  
        public string SroItemSerialNo { get; set; }  
        public string BuyerRegistrationType { get; set; }  
    }
}
