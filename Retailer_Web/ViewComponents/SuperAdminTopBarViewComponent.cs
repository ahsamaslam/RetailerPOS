using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Retailer.Web.Models;
using Retailer.Web.Services.Layout;

namespace Retailer.Web.ViewComponents;

public class SuperAdminTopBarViewComponent : ViewComponent
{
    private readonly ILayoutContext _layout;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SuperAdminTopBarViewComponent(
        ILayoutContext layout,
        IHttpContextAccessor httpContextAccessor)
    {
        _layout = layout;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userInfo = await _layout.GetUserInfoAsync();
        var session = _httpContextAccessor.HttpContext?.Session;

        var companyId = session?.GetString("ImpersonatedCompanyId");
        var companyName = session?.GetString("ImpersonatedCompanyName") ?? "Not selected";

        var viewModel = new SuperAdminTopBarViewModel
        {
            UserInfo = userInfo,
            HasCompanyContext = !string.IsNullOrEmpty(companyId),
            CompanyName = companyName
        };

        return View(viewModel);
    }
}