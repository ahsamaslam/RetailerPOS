namespace Retailer.POS.Api.Entities;
public class StockTransfer : BaseEntity
{
    public DateTime Date { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public string? LoginCode { get; set; }
    public string? FromBranchCode { get; set; }
    public string? ToBranchCode { get; set; }
    public int LoginId { get; set; }
    public ICollection<StockTransferDetail> Details { get; set; } = new List<StockTransferDetail>();
}

public class StockTransferDetail : BaseEntity
{
    public int StockTransferId { get; set; }
    public StockTransfer? StockTransfer { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public decimal Qty { get; set; }
}
