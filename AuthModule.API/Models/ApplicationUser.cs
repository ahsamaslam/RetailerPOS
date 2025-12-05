using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthModule.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        // nullable - user may not belong to a company initially
        public Guid? CompanyId { get; set; }

        // navigation property
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }
    }
}
