using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Repositories;

namespace Retailer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockTransfersController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        public StockTransfersController(IUnitOfWork uow) => _uow = uow;

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _uow.StockTransfers.GetAllAsync());

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StockTransfer model)
        {
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
