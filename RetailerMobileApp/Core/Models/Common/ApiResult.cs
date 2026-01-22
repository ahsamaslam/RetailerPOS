namespace RetailerMobileApp.Core.Models.Common;

public class ApiResult<T>
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public T? Data { get; init; }

    public static ApiResult<T> FromData(T data) => new() { Success = true, Data = data };

    public static ApiResult<T> FromError(string message) => new() { Success = false, ErrorMessage = message };
}
