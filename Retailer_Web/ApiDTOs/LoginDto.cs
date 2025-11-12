namespace Retailer.Web.ApiDTOs
{
    public class LoginDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int EmployeeId { get; set; }
        public int RoleId { get; set; }
    }
}
