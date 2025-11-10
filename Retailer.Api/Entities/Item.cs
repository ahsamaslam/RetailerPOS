namespace Retailer.POS.Api.Entities;
public class Item : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public decimal Rate { get; set; }
    public decimal Cost { get; set; }

    public int CategoryId { get; set; }
    public ItemCategory? Category { get; set; }

    public int ItemTypeId { get; set; }
    public ItemType? ItemType { get; set; }

    public int GroupId { get; set; }
    public ItemGroup? Group { get; set; }

    public int? SubGroupId { get; set; }
    public ItemSubGroup? SubGroup { get; set; }

    public string? UnitName { get; set; }
    public string? UnitCode { get; set; }
    public int? UnitOfMeasureId { get; set; }
    public UnitOfMeasure? UnitOfMeasure { get; set; }
}
