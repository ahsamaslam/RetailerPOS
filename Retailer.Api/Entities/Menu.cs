namespace Retailer.Api.Entities
{
    public class Menu
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Icon { get; set; }        // optional icon class
        public int SortOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;

        // One-to-many SubMenus
        public ICollection<SubMenu> SubMenus { get; set; } = new List<SubMenu>();
    }

}
