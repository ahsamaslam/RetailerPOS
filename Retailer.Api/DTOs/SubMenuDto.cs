namespace Retailer.Api.DTOs
{
    public class SubMenuDto
    {
        public int Id { get; set; }
        public int MenuId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Route { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public List<int> PermissionIds { get; set; } = new(); // ids of permissions applied to this sub menu
        public List<string>? PermissionNames { get; set; }   // optionally populated
    }
}
