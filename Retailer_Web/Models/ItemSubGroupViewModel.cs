namespace Retailer.POS.Web.Models;
public class ItemSubGroupViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int GroupId { get; set; }
    public string? GroupName { get; set; }
}
