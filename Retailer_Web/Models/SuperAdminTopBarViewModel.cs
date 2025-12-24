namespace Retailer.Web.Models
{
    public sealed class SuperAdminTopBarViewModel
    {
        public LayoutUserInfo? UserInfo { get; init; }
        public bool HasCompanyContext { get; init; }
        public string CompanyName { get; init; } = "Not selected";
    }
}
