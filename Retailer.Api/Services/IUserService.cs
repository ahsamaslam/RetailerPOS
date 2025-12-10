using Retailer.Api.DTOs;

namespace Retailer.Api.Services
{
    public interface IUserService
    { 
        Task<UserDto?> GetCurrentUserAsync(); 
        Task<UserResponseDto?> CheckCurrentUserPassword(UserPasswordRequestDto user); 
        Task<UserResponseDto?> ChangePassword(UserPasswordRequestDto user); 
    }
}
