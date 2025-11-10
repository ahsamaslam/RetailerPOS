using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Retailer.Api.DTOs;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Repositories;

namespace Retailer.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        public SalesController(IUnitOfWork uow) => _uow = uow;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSalesDto dto)
        {
            // Map DTO -> entity (simple manual mapping)
            var master = new SalesMaster
            {
                Date = dto.Date,
                CreateDate = DateTime.UtcNow,
                LoginId = dto.LoginId,
                BranchId = dto.BranchId,
                CustomerName = dto.CustomerName,
                SubTotal = dto.SubTotal,
                TotalDiscount = dto.TotalDiscount,
                TaxAmount = dto.TaxAmount,
                BalanceAmount = dto.BalanceAmount,
                CustomerCode = dto.CustomerCode
            };

            await _uow.SalesMasters.AddAsync(master);
            await _uow.SaveChangesAsync();

            foreach (var d in dto.Details)
            {
                var sd = new SalesDetail
                {
                    SalesMasterId = master.Id,
                    ItemCode = d.ItemCode,
                    ItemName = d.ItemName,
                    Rate = d.Rate,
                    Qty = d.Qty,
                    Discount = d.Discount,
                    TaxPercentage = d.TaxPercentage,
                    TaxAmount = d.TaxAmount,
                    Amount = d.Amount
                };
                await _uow.SalesDetails.AddAsync(sd);
            }

            await _uow.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = master.Id }, master);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var master = await _uow.SalesMasters.Query()
                .FirstOrDefaultAsync(s => s.Id == id);
            if (master == null) return NotFound();

            var details = await _uow.SalesDetails.Query()
                .Where(d => d.SalesMasterId == id)
                .ToListAsync();

            return Ok(new { master, details });
        }
    }
}