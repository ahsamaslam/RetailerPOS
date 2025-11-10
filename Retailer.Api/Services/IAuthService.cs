using Retailer.POS.Api.DTOs;
namespace Retailer.POS.Api.Services;
public interface IAuthService
{
    Task<string?> ValidateAndCreateTokenAsync(string username, string password);
}
