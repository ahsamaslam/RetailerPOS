namespace Retailer.POS.Api.Entities;
public class Branch : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? ContactPerson { get; set; }
    public string? MobileNo { get; set; }
    public string? BillHeading1 { get; set; }
    public string? BillHeading2 { get; set; }
    public string? BillMobileNo { get; set; }
}
