using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Retailer.Web.Pages
{
    public abstract class BasePageModel : PageModel
    {
        public override async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            var httpContext = context.HttpContext;

            if (httpContext.User?.Identity?.IsAuthenticated == true &&
                httpContext.User.IsInRole("superadmin"))
            {
                var requestPath = httpContext.Request.Path.Value ?? string.Empty;

                if (!IsSuperAdminBypassPath(requestPath))
                {
                    var impersonatedCompany = httpContext.Session?.GetString("ImpersonatedCompanyId");
                    if (string.IsNullOrEmpty(impersonatedCompany))
                    {
                        context.Result = new RedirectToPageResult("/SuperAdmin/SwitchCompany");
                        return;
                    }
                }
            }

            await next();
        }

        private static bool IsSuperAdminBypassPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            return path.StartsWith("/Login", StringComparison.OrdinalIgnoreCase) ||
                   path.StartsWith("/SuperAdmin/SwitchCompany", StringComparison.OrdinalIgnoreCase) ||
                   path.StartsWith("/SuperAdmin/SearchCompanies", StringComparison.OrdinalIgnoreCase);
        }
    }
}