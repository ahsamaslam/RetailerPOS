namespace Retailer.Web.ApiDTOs;

public class UploadDataResultDto
{
    public int TotalRows { get; set; }
    public int CategoriesCreated { get; set; }
    public int GroupsCreated { get; set; }
    public int SubGroupsCreated { get; set; }
    public int ItemTypesCreated { get; set; }
    public int ItemsCreated { get; set; }
    public int ItemsUpdated { get; set; }
    public int RowsSkipped { get; set; }
    public List<string> Errors { get; set; } = new();
}
