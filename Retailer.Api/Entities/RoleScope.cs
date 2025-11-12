using Retailer.POS.Api.Entities;
using System.Data;

namespace Retailer.Api.Entities
{
    public class RoleScope:BaseEntity
    {
        public int RoleId { get; set; }
        public Role? Role { get; set; }

        public int ScopeId { get; set; }
        public Scope? Scope { get; set; }
    }
}
