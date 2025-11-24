namespace Retailer.Web.ApiDTOs
{
    public class MenuDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public List<SubMenuDto> SubMenus { get; set; } = new();
    }

    public class SubMenuDto
    {
        public int Id { get; set; }
        public int MenuId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Route { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public List<string>? PermissionNames { get; set; }
    }
}
