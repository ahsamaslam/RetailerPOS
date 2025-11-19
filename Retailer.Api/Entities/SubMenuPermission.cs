namespace Retailer.Api.Entities
{
    public class SubMenuPermission
    {
        public int SubMenuId { get; set; }
        public SubMenu? SubMenu { get; set; }

        // just an id — do NOT include a Permission navigation
        public string PermissionName { get; set; } // Use string instead of foreign key to Permission entity
    }

}
