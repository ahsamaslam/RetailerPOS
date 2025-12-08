namespace Retailer.API.Services
{
    public interface IDbInitializer
    {
        /// <summary>
        /// Ensure default roles and permissions exist and are assigned.
        /// Idempotent - safe to call multiple times.
        /// </summary>
        Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken = default);
    }
}
