namespace Retailer.POS.Api.Entities;
public class ItemCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public ICollection<Item> Items { get; set; } = new List<Item>();
}
