using Retailer.POS.Api.Entities;

namespace Retailer.Api.Entities
{
    public class Scope : BaseEntity
    {
        public string ScopeName { get; set; } = string.Empty;
        public ICollection<RoleScope> RoleScopes { get; set; } = new List<RoleScope>();
    }
}
