using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Retailer.POS.Web.Services;
using Retailer.Web.ApiDTOs;
using Retailer.Web.Pages;
using System;

namespace Retailer.POS.Web.Pages.Companies
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly IApiClient _api;
        public DetailsModel(IApiClient api)
        {
            _api = api;
        }
        public CompanyDto? Company { get; private set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var dto = await _api.GetCompanyByIdAsync(id);
            if (dto == null) return NotFound();

            Company = dto;
            return Page();
        }
    }
}
