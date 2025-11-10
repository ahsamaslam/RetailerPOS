namespace Retailer.POS.Api.Entities;
public class ItemGroup : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public ICollection<ItemSubGroup> SubGroups { get; set; } = new List<ItemSubGroup>();
    public ICollection<Item> Items { get; set; } = new List<Item>();
}
