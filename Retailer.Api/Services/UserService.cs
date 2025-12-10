using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Retailer.Api.DTOs;
using Retailer.Api.Entities;
using Retailer.POS.Api.Data;
using System.Net.Http;

namespace Retailer.Api.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly RetailerDbContext _db;
        private readonly IMemoryCache _cache;

        public UserService(
            RetailerDbContext db,
            IMemoryCache cache,
            IHttpClientFactory httpClientFactory)
        {
            _db = db;
            _cache = cache;

            // get the named client
            _httpClient = httpClientFactory.CreateClient("AuthModule");
        }

        public async Task<UserResponseDto?> ChangePassword(UserPasswordRequestDto user)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            var response = await _httpClient.PostAsJsonAsync("api/user/ChangePassword", user);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException("Failed to get User in AuthModule");

            var createdCompany = await response.Content.ReadFromJsonAsync<UserResponseDto>();

            return createdCompany;
        }

        public async Task<UserResponseDto?> CheckCurrentUserPassword(UserPasswordRequestDto user)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            var response = await _httpClient.PostAsJsonAsync("api/User/CheckCurrentUserPassword", user);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException("Failed to create Company in AuthModule");

            var createdCompany = await response.Content.ReadFromJsonAsync<UserResponseDto>();

            return createdCompany;
        }

        // Admin: full list
        public async Task<UserDto> GetCurrentUserAsync()
        {
            var response = await _httpClient.GetAsync($"api/User/currentUser");
            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException("Failed to fetch Companies from AuthModule");

            var currentUser = await response.Content.ReadFromJsonAsync<UserDto>();
            return currentUser;
        }

        

    }
}
