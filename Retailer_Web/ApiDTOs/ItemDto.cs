namespace Retailer.POS.Web.DTOs;
public class ItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public decimal Rate { get; set; }
    public decimal Cost { get; set; }
    public string? CategoryName { get; set; }
    public string? GroupName { get; set; }
    public string? SubGroupName { get; set; }
    public string? ItemTypeName { get; set; }
    public int CategoryId { get; set; }
    public int GroupId { get; set; }
    public int? SubGroupId { get; set; }
    public int ItemTypeId { get; set; }
}
