namespace Retailer.Web.Models
{
    public class LayoutUserInfo
    {
        public string UserName { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = "/assets/img/user2-160x160.jpg";
        public bool IsAdmin { get; set; }
        public bool IsSuperAdmin { get; set; }
        public bool HasCompanyContext { get; set; }
        public string? companyName { get; set; }
        public string? picture { get; set; }

        public string CompanyDisplayName => string.IsNullOrWhiteSpace(companyName)
            ? "Select a company"
            : companyName!;

        public bool RequiresCompanySelection => IsSuperAdmin && !HasCompanyContext;
    }
}
