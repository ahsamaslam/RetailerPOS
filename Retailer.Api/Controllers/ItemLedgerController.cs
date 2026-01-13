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
    public class ItemLedgerController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly RetailerDbContext _context;
        public ItemLedgerController(IUnitOfWork uow, RetailerDbContext context)
        {
            _uow = uow; 
            _context = context;
        }
        

        private Guid CompanyId => HttpContext.GetCompanyId();
		[HttpGet("Ledger/{sdate}/{edate}/{customerid}")]
		public async Task<IActionResult> Get(DateTime sdate, DateTime edate, int customerid)
		{
			try
			{
				var ledgerService = new ItemLedgerService(_context);
				var balance = await ledgerService.GetItemLedgerAsync(customerid, sdate, edate);
				return Ok(balance);
			}
			catch (Exception exx)
			{ return BadRequest(exx.Message); }
		}

		[HttpGet("{edate}/{customerid}")]
        public async Task<IActionResult> Get( DateTime edate, int customerid)
        {
            try
            {
                var ledgerService = new ItemLedgerService(_context);
            var balance=    await ledgerService.GetItemClosingBalanceAsync(edate, customerid);
                return Ok(balance);
            }
            catch (Exception exx)
            { return BadRequest(exx.Message); }
        }

         

    }
}
