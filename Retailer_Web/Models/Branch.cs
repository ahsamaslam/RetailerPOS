namespace Retailer.POS.Web.Models;
public class Branch
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? ContactPerson { get; set; }
    public string? MobileNo { get; set; }
}
