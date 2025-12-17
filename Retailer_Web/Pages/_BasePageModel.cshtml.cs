using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.Web.ApiDTOs;
using Retailer.Web.Helpers;
using Retailer.Web.Models;
using Retailer.Web.Services.Layout;

namespace Retailer.Web.Pages
{
	public abstract class BasePageModel : PageModel
	{
        //public LayoutUserInfo UserInfo { get; private set; } = null!;
        //public IEnumerable<MenuDto> Menus { get; private set; } = Enumerable.Empty<MenuDto>();

        public BasePageModel()
        {
            //_layout = layout;
        }

        public override async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            //try
            //{
            //    UserInfo = await _layout.GetUserInfoAsync();
            //    Menus = await _layout.GetMenusAsync();
            //}
            //catch (ApiUnauthorizedException)
            //{
            //    context.Result = new RedirectToPageResult("/Login");
            //    return;
            //}
            //catch (UnauthorizedAccessException)
            //{
            //    context.Result = new RedirectToPageResult("/Login");
            //    return;
            //}

            await next();
        }
	}
}
