namespace Retailer.POS.Api.Entities;
public class SalesMaster : BaseEntity
{
    public DateTime Date { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public int LoginId { get; set; }
    public int BranchId { get; set; }
    public string? CustomerName { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal BalanceAmount { get; set; }
    public string? CustomerCode { get; set; }
    public ICollection<SalesDetail> Details { get; set; } = new List<SalesDetail>();
}

public class SalesDetail : BaseEntity
{
    public int SalesMasterId { get; set; }
    public SalesMaster? SalesMaster { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public decimal Qty { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxPercentage { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Amount { get; set; }
}
