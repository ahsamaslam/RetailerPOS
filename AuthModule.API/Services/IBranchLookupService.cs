namespace AuthModule.API.Services;

public interface IBranchLookupService
{
    Task<IDictionary<int, string>> GetNamesAsync(IEnumerable<int> branchIds, Guid companyId, CancellationToken cancellationToken = default);
}
