using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Services;
using Retailer.Web.ApiDTOs;
using Retailer.Web.Helpers;
using Retailer.Web.Models;
using System.Security.Claims;

namespace Retailer.Web.Pages
{
	public abstract class BasePageModel : PageModel
	{
		protected const string LayoutUserInfoKey = "_LayoutUserInfo";
        protected readonly IApiClient _api;

        protected BasePageModel(IApiClient api)
        {
            _api = api;
        }
        //public override async Task OnPageHandlerExecutionAsync(Microsoft.AspNetCore.Mvc.Filters.PageHandlerExecutingContext context, Microsoft.AspNetCore.Mvc.Filters.PageHandlerExecutionDelegate next)
        //{
        //    if (!HttpContext.Items.ContainsKey(LayoutUserInfoKey))
        //    {
        //        var info = new LayoutUserInfo();

        //        if (User?.Identity?.IsAuthenticated == true)
        //        {
        //            // Try common claim names; fallback to Identity.Name
        //            info.UserName = User.FindFirst("name")?.Value
        //                            ?? User.FindFirst(ClaimTypes.Name)?.Value
        //                            ?? User.Identity?.Name
        //                            ?? "User";

        //            info.AvatarUrl = User.FindFirst("picture")?.Value
        //                             ?? User.FindFirst("avatar")?.Value
        //                             // use Url.Content to make sure ~ works when rendering
        //                             ?? Url.Content("~/assets/img/user2-160x160.jpg");

        //            // Role name is usually "Admin" (case-sensitive depending on your store)
        //            info.IsAdmin = User.IsInRole("admin");
        //        }

        //        // store in HttpContext.Items so layout/partial can retrieve it (short-lived per-request)
        //        HttpContext.Items[LayoutUserInfoKey] = info;
        //    }

        //    // Only populate if not already present
        //    if (!HttpContext.Items.ContainsKey("Menus"))
        //    {
        //        try
        //        {
        //            var menus = await _api.GetMenusForCurrentUserAsync();
        //            HttpContext.Items["Menus"] = menus ?? Enumerable.Empty<MenuDto>();
        //        }
        //        catch
        //        {
        //            HttpContext.Items["Menus"] = Enumerable.Empty<MenuDto>();
        //        }
        //    }

        //    await next();
        //}
        public override async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context,PageHandlerExecutionDelegate next)
        {
            try
            {
                PopulateUserInfoAsync();
                await PopulateMenusAsync();
            }
            catch (ApiUnauthorizedException)
            {
                context.Result = new RedirectToPageResult("/Login");
                return;
            }
            catch (UnauthorizedAccessException)
            {
                context.Result = new RedirectToPageResult("/Login");
                return;
            }

            await next();
        }
        protected async Task PopulateUserInfoAsync()
        {
            var info = new LayoutUserInfo();
            var company =await _api.GetUserCompanyAsync();
            if (User?.Identity?.IsAuthenticated == true)
            {
                // Try common claim names; fallback to Identity.Name
                info.UserName = User.FindFirst("name")?.Value
                                ?? User.FindFirst(ClaimTypes.Name)?.Value
                                ?? User.Identity?.Name
                                ?? "User";

                info.AvatarUrl = User.FindFirst("picture")?.Value;

                // Role name is usually "Admin" (case-sensitive depending on your store)
                info.IsAdmin = User.IsInRole("admin");
            }
            info.companyName = company.Name;
            // store in HttpContext.Items so layout/partial can retrieve it (short-lived per-request)
            HttpContext.Items[LayoutUserInfoKey] = info;
        }
        protected async Task PopulateMenusAsync()
        {
            if (!HttpContext.Items.ContainsKey("Menus"))
            {
                var menus = await _api.GetMenusForCurrentUserAsync();
                HttpContext.Items["Menus"] = menus ?? Enumerable.Empty<MenuDto>();
            }
        }
        /// <summary>
        /// Helper to get LayoutUserInfo from current request
        /// </summary>
        protected LayoutUserInfo GetLayoutUserInfo()
		{
			return HttpContext.Items[LayoutUserInfoKey] as LayoutUserInfo
				   ?? new LayoutUserInfo();
		}
	}
}
