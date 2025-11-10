namespace Retailer.POS.Web.Models;
public class Vendor
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? CNIC { get; set; }
    public string? NTN { get; set; }
    public string? Mobile { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
}
