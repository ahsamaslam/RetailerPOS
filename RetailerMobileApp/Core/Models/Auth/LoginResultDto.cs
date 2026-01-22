namespace RetailerMobileApp.Core.Models.Auth;

public record LoginResultDto(string Token, double ExpiresIn, IReadOnlyList<string> Roles, string? RefreshToken = null);
