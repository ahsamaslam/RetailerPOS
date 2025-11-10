using Microsoft.AspNetCore.Mvc;
using Retailer.POS.Api.DTOs;
using Retailer.POS.Api.Services;

namespace Retailer.POS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchasesController : ControllerBase
{
    private readonly IPurchaseService _svc;
    public PurchasesController(IPurchaseService svc) => _svc = svc;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePurchaseDto dto)
    {
        var created = await _svc.CreatePurchaseAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var pm = await _svc.GetByIdAsync(id);
        if (pm == null) return NotFound();
        return Ok(pm);
    }
}
