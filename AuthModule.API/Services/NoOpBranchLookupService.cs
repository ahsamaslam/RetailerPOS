namespace AuthModule.API.Services;

public sealed class NoOpBranchLookupService : IBranchLookupService
{
    public Task<IDictionary<int, string>> GetNamesAsync(IEnumerable<int> branchIds, Guid companyId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IDictionary<int, string>>(new Dictionary<int, string>());
    }
}
