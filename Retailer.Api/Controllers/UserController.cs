using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Retailer.Api.DTOs;
using Retailer.Api.Services;
using System.Security.Claims;

namespace Retailer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService) => _userService = userService;
 
        
        [HttpGet("currentUser")]
        public async Task<IActionResult> GetCurrentUserAsync() => Ok(await _userService.GetCurrentUserAsync());
        
        [HttpPost("currentUserPassword")]
        public async Task<UserResponseDto?> CheckCurrentUserPassword([FromForm]UserPasswordRequestDto user)
        {
            return await _userService.CheckCurrentUserPassword(user);
        }
        [HttpPost("ChangePassword")]
        public async Task<UserResponseDto?> ChangePassword([FromForm] UserPasswordRequestDto user)
        {
          return  await _userService.ChangePassword(user);

        }


    }

}
