namespace Retailer.Api.Entities
{
    public class SubMenu
    {
        public int Id { get; set; }
        public int MenuId { get; set; }
        public Menu? Menu { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? Route { get; set; }       // e.g. "/items", "items/index"
        public string? Icon { get; set; }
        public int SortOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;

        // many-to-many to Permission (a submenu can require multiple permissions)
        public ICollection<SubMenuPermission> SubMenuPermissions { get; set; } = new List<SubMenuPermission>();
    }

}
