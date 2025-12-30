using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Retailer.Api.DTOs;
using Retailer.Api.Entities;
using Retailer.Api.Infrastructure;
using Retailer.Api.Services;
using Retailer.POS.Api.Data;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Repositories;

namespace Retailer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class VendorLedgerController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly RetailerDbContext _context;
        public VendorLedgerController(IUnitOfWork uow, RetailerDbContext context)
        {
            _uow = uow; 
            _context = context;
        }
        

        private Guid CompanyId => HttpContext.GetCompanyId();

       
        [HttpGet("{edate}/{id}")]
        public async Task<IActionResult> Get( DateTime edate, int id)
        {
            try
            {
                var ledgerService = new VendorLedgerService(_context);
            var balance=    await ledgerService.GetVendorClosingBalanceAsync(edate, id);
                return Ok(balance);
            }
            catch (Exception exx)
            { return BadRequest(exx.Message); }
        }

         

    }
}
