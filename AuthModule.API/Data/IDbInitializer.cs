namespace AuthModule.API.Data
{
    public interface IDbInitializer
    {
        /// <summary>
        /// Ensure default roles and permissions exist and are assigned.
        /// Idempotent - safe to call multiple times.
        /// </summary>
        Task InitializeAsync(CancellationToken cancellationToken = default);
    }
}
