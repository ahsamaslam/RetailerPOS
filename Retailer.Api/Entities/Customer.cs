using Retailer.Api.Entities;
using Retailer.POS.Api.Entities;

namespace Retailer.POS.Api.Entities;

public class Customer : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? CNIC { get; set; }
    public string? NTN { get; set; }
    public string? STRN { get; set; }
    public string? Mobile { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public int? CityId { get; set; }
    public Cities? City { get; set; }
    public string? Province { get; set; }
    public bool? Register { get; set; }
    public PaymentType PaymentType { get; set; }
    public double openingBalance { get; set; } = 0;
    public DateTime? openDate { get; set; }

  
}
public enum PaymentType
{
    Cash,
    Credit
}