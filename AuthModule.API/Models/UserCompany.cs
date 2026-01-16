using System.ComponentModel.DataAnnotations.Schema;

namespace AuthModule.API.Models
{
    public class UserCompany
    {
        public string UserId { get; set; } = null!;
        public Guid CompanyId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;

        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; } = null!;
    }
}
