namespace Retailer.POS.Api.DTOs;

public class UploadStockResultDto
{
    public int TotalRows { get; set; }
    public int StockAdd { get; set; }
    public int StockUpdated { get; set; }
    public int StockDeleted { get; set; } 
    public int RowsSkipped { get; set; }
    public List<string> Errors { get; set; } = new();
}
