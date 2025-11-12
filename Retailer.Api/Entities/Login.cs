using Retailer.Api.Entities;

namespace Retailer.POS.Api.Entities;
public class Login : BaseEntity
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
}
