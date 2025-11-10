namespace Retailer.POS.Api.Entities;
public class ItemSubGroup : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int GroupId { get; set; }
    public ItemGroup? Group { get; set; }
    public ICollection<Item> Items { get; set; } = new List<Item>();
}
