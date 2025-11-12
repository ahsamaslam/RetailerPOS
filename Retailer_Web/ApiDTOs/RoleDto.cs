namespace Retailer.POS.Web.ApiDTOs
{
    public class RoleDto
    {
        public int Id { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public List<int> ScopeIds { get; set; } = new();
    }
}
