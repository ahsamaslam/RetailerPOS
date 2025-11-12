using Retailer.POS.Api.Entities;

namespace Retailer.Api.Entities
{
    public class Role : BaseEntity
    {
        public string RoleName { get; set; } = string.Empty;
        public ICollection<Login> Logins { get; set; } = new List<Login>();
        public ICollection<RoleScope> RoleScopes { get; set; } = new List<RoleScope>();
    }
}
