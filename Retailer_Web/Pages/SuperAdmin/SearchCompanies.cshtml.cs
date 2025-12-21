using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Reporting.Map.WebForms.BingMaps;
using System.Net;

namespace Retailer.Web.Pages.SuperAdmin
{
    [Authorize(Roles = "superadmin")]
    public class SearchCompaniesModel : PageModel
    {
        private readonly IHttpClientFactory _factory;

        public SearchCompaniesModel(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        public async Task<IActionResult> OnGetAsync(string q)
        {
            var client = _factory.CreateClient("AuthApi");
            var res = await client.GetAsync($"api/companies/search?q={q}");
            if (res.StatusCode == HttpStatusCode.Unauthorized)
                return Unauthorized();

            if (!res.IsSuccessStatusCode)
                return StatusCode((int)res.StatusCode);

            res.EnsureSuccessStatusCode();
            return new JsonResult(
                await res.Content.ReadFromJsonAsync<object>());
        }
    }
}
