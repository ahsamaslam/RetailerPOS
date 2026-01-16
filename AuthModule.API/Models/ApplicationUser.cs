using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthModule.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string picture { get; set; } = "/assets/img/user2-160x160.jpg";
        // nullable - user may not belong to a company initially
        public Guid? CompanyId { get; set; }
        public int? BranchId { get; set; }

        // navigation property
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }
        public ICollection<UserCompany> UserCompanies { get; set; } = new List<UserCompany>();
    }
}
