using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Retailer.Api.Entities;
using Retailer.Api.Services;
using Retailer.POS.Api.Data;
using Retailer.POS.Api.DTOs;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Repositories;
using Retailer.POS.API.UnitOfWork;
using System.Globalization;
using System.Text;

namespace Retailer.POS.Api.Services;

public class UploadStockService : IUploadStockService
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<UploadStockService> _logger;
    private readonly ItemLedgerService _service; 

    public UploadStockService(IUnitOfWork uow, ILogger<UploadStockService> logger, RetailerDbContext context)
    {
        _uow = uow;
        _logger = logger;
        _service =  new ItemLedgerService(context);
    }
    private async Task<int?> GetItemIdAsync(string name, string category, string itemType, string group, string? subGroup, Guid companyId)
    {
        var query = _uow.Items.Query(i => i.CompanyId == companyId);

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(i => i.Name == name.Trim());

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(i => i.Category.Name == category.Trim());

        if (!string.IsNullOrWhiteSpace(itemType))
            query = query.Where(i => i.ItemType.Name == itemType.Trim());

        if (!string.IsNullOrWhiteSpace(group))
            query = query.Where(i => i.Group.Name == group.Trim());

        if (!string.IsNullOrWhiteSpace(subGroup))
            query = query.Where(i => i.SubGroup.Name == subGroup.Trim());

        var item = await query.FirstOrDefaultAsync();
        return item?.Id;
    }
     public async Task<UploadStockResultDto> ImportAsync(Stream stream, string fileName, Guid companyId, CancellationToken cancellationToken)
     {
         if (stream == null) throw new ArgumentNullException(nameof(stream));
     
         var rows = await ParseOpeningBalanceCsvAsync(stream, cancellationToken);
         var result = new UploadStockResultDto
         {
             TotalRows = rows.Count
         };
     
         if (rows.Count == 0)
         {
             _logger.LogWarning("Upload file {FileName} did not contain data rows", fileName);
             return result;
         }
     
          
         

    foreach (var row in rows)
{
    // Get ItemID by combination
    var itemId = await GetItemIdAsync(row.ItemName, row.CategoryName, row.ItemTypeName, row.GroupName, row.SubGroupName, companyId);
    if (itemId == null)
    {
        result.Errors.Add($"Row {row.RowNumber}: Could not find Item with given combination.");
        continue; // skip invalid row
    }
            
    var openingBalance = new OpeningBalance
    {
        Year = DateTime.UtcNow.Year,
        BranchId = row.BranchId,
        ProductID = itemId.Value,
        OpeningQuantity = row.Quantity,
        CreatedAt = DateTime.UtcNow,
        CompanyId= companyId,
        Id = itemId.Value
    };
            try
            {
                var existing = await _uow.OpeningBalances.Query().FirstOrDefaultAsync(x =>
    x.Year == openingBalance.Year &&
    x.BranchId == openingBalance.BranchId &&
    x.ProductID == openingBalance.ProductID);

                if (existing == null)
                {
                    // ➕ ADD
                    

                    await _uow.OpeningBalances.AddAsync(openingBalance);
                    await _uow.SaveChangesAsync();
                    await _service.PostLedgerAsync(openingBalance);
                }
                else
                {
                    // ✏️ UPDATE
                    existing.OpeningQuantity = row.Quantity;
                 //   existing.UpdatedAt = DateTime.UtcNow;

                    _uow.OpeningBalances.Update(existing);
                    await _uow.SaveChangesAsync();
                    existing.odlQuantity = existing.OpeningQuantity;
                    await _service.UpdateLedgerAsync(existing);
                }

               
            }
            catch (Exception ex)
            { 
            
            }
        }
        //_//logger.LogInformation("Upload completed for company {CompanyId}. Created {Created} items, updated {Updated} items, skipped {Skipped} rows", companyId, result.ItemsCreated, result.ItemsUpdated, result.RowsSkipped);
        return result;
}

private async Task<Dictionary<string, ItemCategory>> BuildCategoryLookupAsync(Guid companyId)
    {
        var categories = await _uow.ItemCategories.GetAllAsync(c => c.CompanyId == companyId);
        return categories
            .GroupBy(c => NormalizeKey(c.Name))
            .Where(g => !string.IsNullOrEmpty(g.Key))
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
    }

    private async Task<Dictionary<string, ItemGroup>> BuildGroupLookupAsync(Guid companyId)
    {
        var groups = await _uow.ItemGroups.GetAllAsync(g => g.CompanyId == companyId);
        return groups
            .GroupBy(g => NormalizeKey(g.Name))
            .Where(g => !string.IsNullOrEmpty(g.Key))
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
    }

    private async Task<Dictionary<string, ItemType>> BuildItemTypeLookupAsync(Guid companyId)
    {
        var itemTypes = await _uow.ItemTypes.GetAllAsync(t => t.CompanyId == companyId);
        return itemTypes
            .GroupBy(t => NormalizeKey(t.Name))
            .Where(g => !string.IsNullOrEmpty(g.Key))
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
    }

    private async Task<Dictionary<string, ItemSubGroup>> BuildSubGroupLookupAsync(Guid companyId, Dictionary<int, ItemGroup> groupById)
    {
        var subGroups = await _uow.ItemSubGroups.GetAllAsync(s => s.CompanyId == companyId);
        var lookup = new Dictionary<string, ItemSubGroup>(StringComparer.OrdinalIgnoreCase);

        foreach (var sub in subGroups)
        {
            if (groupById.TryGetValue(sub.GroupId, out var parent))
            {
                var key = BuildSubGroupKey(parent.Name, sub.Name);
                if (!lookup.ContainsKey(key))
                {
                    lookup[key] = sub;
                }
            }
        }

        return lookup;
    }

    private async Task<(Dictionary<string, Item> ByName, Dictionary<string, Item> BySku)> BuildItemLookupsAsync(Guid companyId)
    {
        var items = await _uow.Items.GetAllAsync(i => i.CompanyId == companyId);
        var byName = new Dictionary<string, Item>(StringComparer.OrdinalIgnoreCase);
        var bySku = new Dictionary<string, Item>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in items)
        {
            var nameKey = NormalizeKey(item.Name);
            if (!string.IsNullOrEmpty(nameKey) && !byName.ContainsKey(nameKey))
            {
                byName[nameKey] = item;
            }

            if (!string.IsNullOrWhiteSpace(item.Barcode))
            {
                var skuKey = NormalizeKey(item.Barcode);
                if (!string.IsNullOrEmpty(skuKey) && !bySku.ContainsKey(skuKey))
                {
                    bySku[skuKey] = item;
                }
            }
        }

        return (byName, bySku);
    }

    private async Task<ItemCategory> GetOrCreateCategoryAsync(string name, Guid companyId, Dictionary<string, ItemCategory> cache, UploadDataResultDto result)
    {
        var key = NormalizeKey(name);
        if (!string.IsNullOrEmpty(key) && cache.TryGetValue(key, out var existing))
        {
            return existing;
        }

        var entity = new ItemCategory
        {
            Name = name,
            CompanyId = companyId
        };

        await _uow.ItemCategories.AddAsync(entity);
        if (!string.IsNullOrEmpty(key)) cache[key] = entity;
        result.CategoriesCreated++;
        return entity;
    }

    private async Task<ItemGroup> GetOrCreateGroupAsync(string name, Guid companyId, Dictionary<string, ItemGroup> cache, UploadDataResultDto result)
    {
        var key = NormalizeKey(name);
        if (!string.IsNullOrEmpty(key) && cache.TryGetValue(key, out var existing))
        {
            return existing;
        }

        var entity = new ItemGroup
        {
            Name = name,
            CompanyId = companyId
        };

        await _uow.ItemGroups.AddAsync(entity);
        if (!string.IsNullOrEmpty(key)) cache[key] = entity;
        result.GroupsCreated++;
        return entity;
    }

    private async Task<ItemType> GetOrCreateItemTypeAsync(string name, Guid companyId, Dictionary<string, ItemType> cache, UploadDataResultDto result)
    {
        var key = NormalizeKey(name);
        if (!string.IsNullOrEmpty(key) && cache.TryGetValue(key, out var existing))
        {
            return existing;
        }

        var entity = new ItemType
        {
            Name = name,
            CompanyId = companyId
        };

        await _uow.ItemTypes.AddAsync(entity);
        if (!string.IsNullOrEmpty(key)) cache[key] = entity;
        result.ItemTypesCreated++;
        return entity;
    }

    private async Task<ItemSubGroup?> GetOrCreateSubGroupAsync(string? name, ItemGroup group, Guid companyId, Dictionary<string, ItemSubGroup> cache, UploadDataResultDto result)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        var key = BuildSubGroupKey(group.Name, name);
        if (!string.IsNullOrEmpty(key) && cache.TryGetValue(key, out var existing))
        {
            return existing;
        }

        var entity = new ItemSubGroup
        {
            Name = name.Trim(),
            Group = group,
            GroupId = group.Id,
            CompanyId = companyId
        };

        await _uow.ItemSubGroups.AddAsync(entity);
        if (!string.IsNullOrEmpty(key)) cache[key] = entity;
        result.SubGroupsCreated++;
        return entity;
    }

    private static bool TryParseDecimal(string? value, out decimal result)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result = 0;
            return true;
        }

        return decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result)
            || decimal.TryParse(value, NumberStyles.Float, CultureInfo.CurrentCulture, out result);
    }

    private static string NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static string NormalizeKey(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim().ToLowerInvariant();
    }

    private static string BuildSubGroupKey(string groupName, string? subGroupName)
    {
        if (string.IsNullOrWhiteSpace(subGroupName)) return string.Empty;
        return $"{NormalizeKey(groupName)}::{NormalizeKey(subGroupName)}";
    }

    private async Task<List<ParsedRow>> ParseRowsAsync(Stream stream, CancellationToken cancellationToken)
    {
        if (stream.CanSeek)
        {
            stream.Seek(0, SeekOrigin.Begin);
        }

        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 4096, leaveOpen: true);

        var headerLine = await reader.ReadLineAsync();
        if (headerLine == null)
        {
            throw new InvalidDataException("File is empty.");
        }

        var headers = SplitCsvLine(headerLine);
        var headerMap = BuildHeaderMap(headers);

        if (!headerMap.ContainsKey("ItemName"))
        {
            throw new InvalidDataException("The file must contain an ItemName column.");
        }

        var rows = new List<ParsedRow>();
        string? line;
        var rowNumber = 1;

        while ((line = await reader.ReadLineAsync()) != null)
        {
            cancellationToken.ThrowIfCancellationRequested();
            rowNumber++;

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var values = SplitCsvLine(line);
            if (values.All(string.IsNullOrWhiteSpace))
            {
                continue;
            }

            var row = new ParsedRow
            {
                RowNumber = rowNumber,
                ItemName = GetColumnValue(values, headerMap, "ItemName"),
                Barcode = GetColumnValue(values, headerMap, "Barcode", "Barcode"),
                CategoryName = GetColumnValue(values, headerMap, "Category"),
                GroupName = GetColumnValue(values, headerMap, "Group", "Groups", "GroupName", "Group Name"),
                SubGroupName = GetColumnValue(values, headerMap, "SubGroup", "SubGroups", "Sub Group", "Subgroup", "Subgroups"),
                ItemTypeName = GetColumnValue(values, headerMap, "ItemType", "Item Type", "Type"), 
                Quantity = Convert.ToDecimal(GetColumnValue(values, headerMap, "Quantity", "Qty")),
            };

            rows.Add(row);
        }

        return rows;
    }

    private static List<string> SplitCsvLine(string line)
    {
        var result = new List<string>();
        if (line == null)
        {
            return result;
        }

        var current = new StringBuilder();
        var inQuotes = false;

        for (var i = 0; i < line.Length; i++)
        {
            var ch = line[i];
            if (ch == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                    continue;
                }

                inQuotes = !inQuotes;
                continue;
            }

            if (ch == ',' && !inQuotes)
            {
                result.Add(current.ToString());
                current.Clear();
                continue;
            }

            current.Append(ch);
        }

        result.Add(current.ToString());
        return result;
    }

    private static Dictionary<string, int> BuildHeaderMap(List<string> headers)
    {
        var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < headers.Count; i++)
        {
            var header = headers[i]?.Trim();
            if (string.IsNullOrEmpty(header))
            {
                continue;
            }

            if (!map.ContainsKey(header))
            {
                map.Add(header, i);
            }
        }

        return map;
    }
    private async Task<List<ParsedRow>> ParseOpeningBalanceCsvAsync(Stream stream, CancellationToken cancellationToken)
    {
        if (stream.CanSeek) stream.Seek(0, SeekOrigin.Begin);

        var rows = new List<ParsedRow>();

        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);

        var headerLine = await reader.ReadLineAsync();
        if (headerLine == null) throw new InvalidDataException("File is empty.");

        var headers = headerLine.Split(','); // TSV delimiter in your example
        var headerMap = headers.Select((h, i) => new { h, i })
                               .ToDictionary(x => x.h.Trim(), x => x.i, StringComparer.OrdinalIgnoreCase);

        string? line;
        var rowNumber = 1;

        while ((line = await reader.ReadLineAsync()) != null)
        {
            cancellationToken.ThrowIfCancellationRequested();
            rowNumber++;

            if (string.IsNullOrWhiteSpace(line)) continue;

            var values = line.Split(',');
            var row = new ParsedRow { RowNumber = rowNumber };

            // Validate required columns
            if (!headerMap.ContainsKey("ItemName") || !headerMap.ContainsKey("Barcode") || !headerMap.ContainsKey("Qty"))
            {
                throw new InvalidDataException("File must contain 'ItemName', 'Barcode', and 'Qty' columns.");
            }
            var itemNameIndex = headerMap["ItemName"];
            var barcodeIndex = headerMap["Barcode"];
            var CategoryNameIndex = headerMap["CategoryName"];
            var ItemTypeNameIndex = headerMap["ItemTypeName"];
            var GroupNameIndex = headerMap["GroupName"];
            var SubGroupNameIndex = headerMap["SubGroupName"];
            var qtyIndex = headerMap["Qty"];
            String qtyval = values[qtyIndex];
            row.ItemName = values[itemNameIndex];
            row.CategoryName = values[CategoryNameIndex];
            row.ItemTypeName = values[ItemTypeNameIndex];
            row.CategoryName = values[CategoryNameIndex];
            row.SubGroupName = values[SubGroupNameIndex];
            row.GroupName = values[GroupNameIndex]; 
            row.Quantity = decimal.TryParse(values[qtyIndex], NumberStyles.Float, CultureInfo.InvariantCulture, out var qty) ? qty : 0;
            if (row.Quantity <= 0)
            {
                continue; // skip invalid quantity
            }

            row.BranchId = 1; // optionally get from user input or default branch

            rows.Add(row);
        }

        return rows;
    }

    private class OpeningBalanceCsvRow
    {
        public int RowNumber { get; set; }
        public int ProductID { get; set; }
        public decimal Quantity { get; set; }
        public int BranchId { get; set; }
    }


    private static string? GetColumnValue(List<string> values, Dictionary<string, int> headerMap, params string[] names)
    {
        foreach (var name in names)
        {
            if (headerMap.TryGetValue(name, out var index) && index < values.Count)
            {
                var value = values[index];
                if (!string.IsNullOrEmpty(value))
                {
                    return value.Trim();
                }
            }
        }

        return null;
    }

    private sealed class ParsedRow
    {
        public int RowNumber { get; set; }
        public int ProductID { get; set; }
        public decimal Quantity { get; set; }
        public int BranchId { get; set; }

        public string ItemName { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string ItemTypeName { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public string SubGroupName { get; set; } = string.Empty;
    }
}
