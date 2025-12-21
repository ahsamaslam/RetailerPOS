using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Retailer.Api.Infrastructure;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Repositories;

namespace Retailer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StockTransfersController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        public StockTransfersController(IUnitOfWork uow) => _uow = uow;
        private Guid CompanyId => HttpContext.GetCompanyId();

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _uow.StockTransfers.GetAllAsync(b => b.CompanyId == CompanyId));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StockTransfer model)
        {
            model.CompanyId = CompanyId;
            await _uow.StockTransfers.AddAsync(model);
            await _uow.SaveChangesAsync();

            if (model.Details != null)
            {
                foreach (var d in model.Details)
                {
                    d.StockTransferId = model.Id;
                    await _uow.StockTransferDetails.AddAsync(d);
                }
                await _uow.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(GetAll), new { id = model.Id }, model);
        }
    }
}
