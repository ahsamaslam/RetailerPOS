using Retailer.Api.Entities;

namespace Retailer.POS.Api.Entities;
public class Employee : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? Address { get; set; }
    public string? Mobile1 { get; set; }
    public string? Mobile2 { get; set; }
    public string? CNIC { get; set; }

    public ICollection<Login> Logins { get; set; } = new List<Login>();
}
