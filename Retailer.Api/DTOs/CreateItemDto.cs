namespace Retailer.POS.Api.DTOs;
public class CreateItemDto
{
    public string Name { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public decimal Rate { get; set; }
    public decimal Cost { get; set; }
    public int CategoryId { get; set; }
    public int ItemTypeId { get; set; }
    public int GroupId { get; set; }
    public int? SubGroupId { get; set; }
    public string? UnitName { get; set; }
    public string? UnitCode { get; set; }
    public int? UnitOfMeasureId { get; set; }
}
