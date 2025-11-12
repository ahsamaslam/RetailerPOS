using Microsoft.AspNetCore.Mvc;
using Retailer.Api.DTOs;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Repositories;

[Route("api/[controller]")]
[ApiController]
public class EmployeeController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    public EmployeeController(IUnitOfWork uow) => _uow = uow;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _uow.Employees.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id) => Ok(await _uow.Employees.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EmployeeDto dto)
    {
        var emp = new Employee
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            City = dto.City,
            Province = dto.Province,
            Address = dto.Address,
            Mobile1 = dto.Mobile1,
            Mobile2 = dto.Mobile2,
            CNIC = dto.CNIC
        };
        await _uow.Employees.AddAsync(emp);
        await _uow.SaveChangesAsync();
        return Ok(emp);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] EmployeeDto dto)
    {
        var emp = await _uow.Employees.GetByIdAsync(id);
        if (emp == null) return NotFound();
        emp.FirstName = dto.FirstName;
        emp.LastName = dto.LastName;
        emp.City = dto.City;
        emp.Province = dto.Province;
        emp.Address = dto.Address;
        emp.Mobile1 = dto.Mobile1;
        emp.Mobile2 = dto.Mobile2;
        emp.CNIC = dto.CNIC;
        await _uow.SaveChangesAsync();
        return Ok(emp);
    }
}
