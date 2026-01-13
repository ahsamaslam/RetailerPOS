using System.Globalization;
using System.Text;
using Microsoft.Extensions.Logging;
using Retailer.POS.Api.DTOs;
using Retailer.POS.Api.Entities;
using Retailer.POS.Api.Repositories;

namespace Retailer.POS.Api.Services;

public class UploadDataService : IUploadDataService
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<UploadDataService> _logger;

    public UploadDataService(IUnitOfWork uow, ILogger<UploadDataService> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<UploadDataResultDto> ImportAsync(Stream stream, string fileName, Guid companyId, CancellationToken cancellationToken)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));

        var rows = await ParseRowsAsync(stream, cancellationToken);
        var result = new UploadDataResultDto
        {
            TotalRows = rows.Count
        };

        if (rows.Count == 0)
        {
            _logger.LogWarning("Upload file {FileName} did not contain data rows", fileName);
            return result;
        }

        _logger.LogInformation("Starting upload of {RowCount} rows for company {CompanyId}", rows.Count, companyId);

        var categoryLookup = await BuildCategoryLookupAsync(companyId);
        var groupLookup = await BuildGroupLookupAsync(companyId);
        var groupById = groupLookup.Values.Where(g => g.Id > 0).ToDictionary(g => g.Id);
        var itemTypeLookup = await BuildItemTypeLookupAsync(companyId);
        var subGroupLookup = await BuildSubGroupLookupAsync(companyId, groupById);
        var (itemByName, itemBySku) = await BuildItemLookupsAsync(companyId);

        foreach (var row in rows)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(row.ItemName))
            {
                result.RowsSkipped++;
                result.Errors.Add($"Row {row.RowNumber}: ItemName is required.");
                continue;
            }

            if (!TryParseDecimal(row.Rate, out var rate))
            {
                result.RowsSkipped++;
                result.Errors.Add($"Row {row.RowNumber}: Rate '{row.Rate}' is invalid.");
                continue;
            }

            if (!TryParseDecimal(row.Cost, out var cost))
            {
                result.RowsSkipped++;
                result.Errors.Add($"Row {row.RowNumber}: Cost '{row.Cost}' is invalid.");
                continue;
            }

            if (!TryParseDecimal(row.Quantity, out var quantity))
            {
                result.RowsSkipped++;
                result.Errors.Add($"Row {row.RowNumber}: Quantity '{row.Quantity}' is invalid.");
                continue;
            }

            var categoryName = string.IsNullOrWhiteSpace(row.Category) ? "General" : row.Category.Trim();
            var groupName = string.IsNullOrWhiteSpace(row.Group) ? "General" : row.Group.Trim();
            var itemTypeName = string.IsNullOrWhiteSpace(row.ItemType) ? "Inventory" : row.ItemType.Trim();

            var category = await GetOrCreateCategoryAsync(categoryName, companyId, categoryLookup, result);
            var group = await GetOrCreateGroupAsync(groupName, companyId, groupLookup, result);
            var itemType = await GetOrCreateItemTypeAsync(itemTypeName, companyId, itemTypeLookup, result);
            var subGroup = await GetOrCreateSubGroupAsync(row.SubGroup, group, companyId, subGroupLookup, result);

            var sanitizedSku = NormalizeOptional(row.Sku);
            var itemNameKey = NormalizeKey(row.ItemName);

            Item? item = null;
            if (!string.IsNullOrWhiteSpace(sanitizedSku) && itemBySku.TryGetValue(NormalizeKey(sanitizedSku), out var bySku))
            {
                item = bySku;
            }
            else if (itemByName.TryGetValue(itemNameKey, out var byName))
            {
                item = byName;
            }

            if (item == null)
            {
                item = new Item
                {
                    Name = row.ItemName.Trim(),
                    Barcode = sanitizedSku,
                    Rate = rate,
                    Cost = cost,
                    QtyInHand = quantity,
                    UnitName = NormalizeOptional(row.UnitName),
                    UnitCode = NormalizeOptional(row.UnitCode),
                    CompanyId = companyId,
                    Category = category,
                    CategoryId = category.Id,
                    Group = group,
                    GroupId = group.Id,
                    ItemType = itemType,
                    ItemTypeId = itemType.Id,
                    SubGroup = subGroup,
                    SubGroupId = subGroup?.Id
                };

                await _uow.Items.AddAsync(item);
                itemByName[itemNameKey] = item;
                if (!string.IsNullOrWhiteSpace(sanitizedSku))
                {
                    itemBySku[NormalizeKey(sanitizedSku)] = item;
                }
                result.ItemsCreated++;
            }
            else
            {
                var previousSku = item.Barcode;

                item.Name = row.ItemName.Trim();
                item.Barcode = sanitizedSku;
                item.Rate = rate;
                item.Cost = cost;
                item.QtyInHand = quantity;
                item.UnitName = NormalizeOptional(row.UnitName);
                item.UnitCode = NormalizeOptional(row.UnitCode);
                item.Category = category;
                item.CategoryId = category.Id;
                item.Group = group;
                item.GroupId = group.Id;
                item.ItemType = itemType;
                item.ItemTypeId = itemType.Id;
                item.SubGroup = subGroup;
                item.SubGroupId = subGroup?.Id;

                _uow.Items.Update(item);
                result.ItemsUpdated++;

                if (!string.IsNullOrWhiteSpace(previousSku))
                {
                    itemBySku.Remove(NormalizeKey(previousSku));
                }
                if (!string.IsNullOrWhiteSpace(sanitizedSku))
                {
                    itemBySku[NormalizeKey(sanitizedSku)] = item;
                }

                itemByName[itemNameKey] = item;
            }
        }

        await _uow.SaveChangesAsync();
        _logger.LogInformation("Upload completed for company {CompanyId}. Created {Created} items, updated {Updated} items, skipped {Skipped} rows", companyId, result.ItemsCreated, result.ItemsUpdated, result.RowsSkipped);
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
                Sku = GetColumnValue(values, headerMap, "SKU", "Barcode"),
                Category = GetColumnValue(values, headerMap, "Category"),
                Group = GetColumnValue(values, headerMap, "Group", "Groups", "GroupName", "Group Name"),
                SubGroup = GetColumnValue(values, headerMap, "SubGroup", "SubGroups", "Sub Group", "Subgroup", "Subgroups"),
                ItemType = GetColumnValue(values, headerMap, "ItemType", "Item Type", "Type"),
                Rate = GetColumnValue(values, headerMap, "Rate", "Price"),
                Cost = GetColumnValue(values, headerMap, "Cost"),
                Quantity = GetColumnValue(values, headerMap, "Quantity", "Qty"),
                UnitName = GetColumnValue(values, headerMap, "UnitName"),
                UnitCode = GetColumnValue(values, headerMap, "UnitCode")
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
        public string? ItemName { get; set; }
        public string? Sku { get; set; }
        public string? Category { get; set; }
        public string? Group { get; set; }
        public string? SubGroup { get; set; }
        public string? ItemType { get; set; }
        public string? Rate { get; set; }
        public string? Cost { get; set; }
        public string? Quantity { get; set; }
        public string? UnitName { get; set; }
        public string? UnitCode { get; set; }
    }
}
