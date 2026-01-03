namespace Retailer.Api.Entities
{
    public class SubMenu
    {
        public int Id { get; set; }
        public int MenuId { get; set; }
        public Menu? Menu { get; set; }

        public string Title { get; set; } = string.Empty;
        public string UrlTitle { get; set; } = string.Empty;  // e.g. "Item List"   
        public string? Icon { get; set; }
        public int SortOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;

    }

}
