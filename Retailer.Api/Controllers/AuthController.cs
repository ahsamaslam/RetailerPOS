using Microsoft.AspNetCore.Mvc;
using Retailer.POS.Api.DTOs;
using Retailer.POS.Api.Services;

namespace Retailer.POS.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto req)
    {
        var token = await _auth.ValidateAndCreateTokenAsync(req.Username, req.Password);
        if (token == null) return Unauthorized();
        return Ok(new { token });
    }
}
