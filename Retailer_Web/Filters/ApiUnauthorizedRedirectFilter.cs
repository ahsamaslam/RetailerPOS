using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Retailer.Web.Helpers;

namespace Retailer.Web.Filters
{
    /// <summary>
    /// Catches <see cref="ApiUnauthorizedException"/> thrown from page handlers, signs the user out,
    /// and sends them back to the login page so they can re-authenticate.
    /// </summary>
    public sealed class ApiUnauthorizedRedirectFilter : IAsyncExceptionFilter
    {
        private readonly ILogger<ApiUnauthorizedRedirectFilter> _logger;

        public ApiUnauthorizedRedirectFilter(ILogger<ApiUnauthorizedRedirectFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnExceptionAsync(ExceptionContext context)
        {
            if (context.Exception is not ApiUnauthorizedException)
            {
                return;
            }

            _logger.LogWarning(context.Exception, "API returned 401 while processing {Path}. User will be signed out.", context.HttpContext.Request.Path);

            await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            context.HttpContext.Session.Clear();

            context.Result = new RedirectToPageResult("/Login");
            context.ExceptionHandled = true;
        }
    }
}
