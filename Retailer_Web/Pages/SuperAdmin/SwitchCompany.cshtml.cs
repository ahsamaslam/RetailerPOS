using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Retailer.Web.Pages.SuperAdmin
{
    [Authorize(Roles = "superadmin")]
    public class SwitchCompanyModel : PageModel
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly ILogger<SwitchCompanyModel> _logger;

        public SwitchCompanyModel(IHttpClientFactory httpFactory, ILogger<SwitchCompanyModel> logger)
        {
            _httpFactory = httpFactory;
            _logger = logger;
        }

        public List<CompanyListItem> Companies { get; private set; } = new();
        public string? CurrentCompanyName { get; private set; }
        public bool HasCompanyContext => !string.IsNullOrEmpty(HttpContext.Session.GetString("ImpersonatedCompanyId"));

        public async Task OnGetAsync()
        {
            CurrentCompanyName = HttpContext.Session.GetString("ImpersonatedCompanyName");
            await LoadCompaniesAsync();
        }

        public async Task<IActionResult> OnPostAsync(Guid companyId, string? companyName)
        {
            CurrentCompanyName = HttpContext.Session.GetString("ImpersonatedCompanyName");

            if (companyId == Guid.Empty)
            {
                ModelState.AddModelError(string.Empty, "Select a company before continuing.");
                await LoadCompaniesAsync();
                return Page();
            }

            var resolvedCompany = await GetCompanyAsync(companyId);
            if (resolvedCompany == null)
            {
                ModelState.AddModelError(string.Empty, "Selected company could not be found. Please try again.");
                await LoadCompaniesAsync();
                return Page();
            }

            var displayName = !string.IsNullOrWhiteSpace(resolvedCompany.Name)
                ? resolvedCompany.Name
                : companyName;

            HttpContext.Session.SetString("ImpersonatedCompanyId", resolvedCompany.Id.ToString());

            if (string.IsNullOrWhiteSpace(displayName))
            {
                HttpContext.Session.Remove("ImpersonatedCompanyName");
            }
            else
            {
                HttpContext.Session.SetString("ImpersonatedCompanyName", displayName);
            }

            _logger.LogInformation(
                "Super admin {User} switched to company {CompanyName} ({CompanyId})",
                User.Identity?.Name,
                displayName ?? "Unnamed",
                resolvedCompany.Id);

            return RedirectToPage("/Index");
        }

        private async Task LoadCompaniesAsync()
        {
            try
            {
                var client = _httpFactory.CreateClient("AuthApi");
                var response = await client.GetAsync("api/companies");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to load companies for picker. Status {StatusCode}", response.StatusCode);
                    Companies = new List<CompanyListItem>();
                    return;
                }

                Companies = await response.Content.ReadFromJsonAsync<List<CompanyListItem>>() ?? new List<CompanyListItem>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to load companies for super admin switcher.");
                Companies = new List<CompanyListItem>();
            }
        }

        private async Task<CompanyListItem?> GetCompanyAsync(Guid companyId)
        {
            try
            {
                var client = _httpFactory.CreateClient("AuthApi");
                var response = await client.GetAsync($"api/companies/{companyId}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Lookup for company {CompanyId} failed with {StatusCode}", companyId, response.StatusCode);
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<CompanyListItem>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to resolve company {CompanyId}", companyId);
                return null;
            }
        }

        public class CompanyListItem
        {
            public Guid Id { get; set; }
            public string? Name { get; set; }
            public string? ShortName { get; set; }
            public string? Address { get; set; }
            public string? Province { get; set; }
            public string? ContactPhone { get; set; }
        }
    }
}