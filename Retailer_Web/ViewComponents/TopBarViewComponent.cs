using Microsoft.AspNetCore.Mvc;
using Retailer.POS.Web.Services;
using Retailer.Web.Helpers;
using Retailer.Web.Services.Layout;
using System.IdentityModel.Claims;

namespace Retailer.Web.ViewComponents
{
    public class TopBarViewComponent : ViewComponent
    {
        private readonly ILayoutContext _layout;

        public TopBarViewComponent(ILayoutContext layout)
        {
            _layout = layout;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userInfo = await _layout.GetUserInfoAsync();

            if (userInfo == null)
                return Content(string.Empty); // or minimal topbar

            return View(userInfo);
        }
    }
}
