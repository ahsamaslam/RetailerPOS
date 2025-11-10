namespace Retailer.POS.Web.Models;
public class Employee
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? Mobile1 { get; set; }
}
