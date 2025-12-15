namespace Retailer.Web.ApiDTOs
{
    public class MenuCompanyViewModel
    {
        public IEnumerable<MenuDto>? Menus { get; set; }
        public CompanyDto? Company { get; set; }
    }
}
