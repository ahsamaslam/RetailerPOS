using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Retailer.Api.DTOs;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Repositories;

[Route("api/[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    public LoginController(IUnitOfWork uow) => _uow = uow;

    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate([FromBody] LoginDto dto)
    {
        var user = (await _uow.Logins.Query()
                    .Include(l => l.Employee)
                    .Include(l => l.Role)
                    .FirstOrDefaultAsync(l => l.UserName == dto.UserName && l.Password == dto.Password));

        if (user == null) return Unauthorized("Invalid credentials");

        return Ok(new
        {
            user.Id,
            user.UserName,
            user.Employee.FirstName,
            user.Role.RoleName
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] LoginDto dto)
    {
        var login = new Login
        {
            UserName = dto.UserName,
            Password = dto.Password,
            EmployeeId = dto.EmployeeId,
            RoleId = dto.RoleId
        };
        await _uow.Logins.AddAsync(login);
        await _uow.SaveChangesAsync();
        return Ok(login);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] LoginDto dto)
    {
        var login = await _uow.Logins.GetByIdAsync(id);
        if (login == null) return NotFound();
        login.UserName = dto.UserName;
        login.Password = dto.Password;
        login.EmployeeId = dto.EmployeeId;
        login.RoleId = dto.RoleId;
        await _uow.SaveChangesAsync();
        return Ok(login);
    }
}
