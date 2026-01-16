using AuthModule.API.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthModule.API.Services;

public class BranchLookupService : IBranchLookupService
{
    private readonly RetailerLookupDbContext _db;
    private readonly ILogger<BranchLookupService> _logger;

    public BranchLookupService(RetailerLookupDbContext db, ILogger<BranchLookupService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IDictionary<int, string>> GetNamesAsync(IEnumerable<int> branchIds, Guid companyId, CancellationToken cancellationToken = default)
    {
        if (branchIds == null)
        {
            return new Dictionary<int, string>();
        }

        var ids = branchIds.Where(id => id > 0).Distinct().ToArray();
        if (ids.Length == 0)
        {
            return new Dictionary<int, string>();
        }

        try
        {
            return await _db.Branches
                .Where(b => ids.Contains(b.Id) && b.CompanyId == companyId)
                .ToDictionaryAsync(b => b.Id, b => b.Name, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to resolve branch names for {@Ids}", ids);
            return new Dictionary<int, string>();
        }
    }
}
