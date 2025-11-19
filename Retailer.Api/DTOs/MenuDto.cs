namespace Retailer.Api.DTOs
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
}
