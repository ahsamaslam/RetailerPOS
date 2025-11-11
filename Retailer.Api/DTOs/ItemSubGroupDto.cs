namespace Retailer.Api.DTOs
{
    public class ItemSubGroupDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int GroupId { get; set; }
        public string? GroupName { get; set; }
    }
}
