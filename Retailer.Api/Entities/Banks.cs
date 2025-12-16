using Retailer.Api.Entities;
using Retailer.POS.Api.Entities;

namespace Retailer.POS.Api.Entities;

public class Banks : BaseEntity
{
    public string AccountName { get; set; } = string.Empty;
    public string? AccountNumber { get; set; }
    public string? BrnchName { get; set; }
    public string? BranchCode { get; set; }
    public string? Mobile { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public int? CityId { get; set; }
    public Cities? City { get; set; }
    public string? Province { get; set; }  
    public double openingBalance { get; set; } = 0;
    public DateTime? openDate { get; set; }

  
}
 