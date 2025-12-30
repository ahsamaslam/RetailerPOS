namespace Retailer.POS.Api.Entities;
public class SalesMaster : BaseEntity
{
    public DateTime Date { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public Guid UserId { get; set; }
    public int BranchId { get; set; }
    public string? CustomerName { get; set; }
    public string? SaleType { get; set; }
    public string? hsCode { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal BalanceAmount { get; set; }
    public decimal totalAmount { get; set; }
    public int? CustomerCode { get; set; }
    public bool Active { get; set; } = true;
    public int Year { get; set; } = 1;
    public int saleCode { get; set; } = 1;
    public string? remarks { get; set; }
    public ICollection<SalesDetail> Details { get; set; } = new List<SalesDetail>();
}

public class SalesDetail : BaseEntity
{
    public int SalesMasterId { get; set; }
    public SalesMaster? SalesMaster { get; set; }
    public string? sroScheduleNo { get; set; }
    public string? uoM { get; set; }
    public string? hsCode { get; set; }
    public int ItemCode { get; set; } 
    public string ItemName { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public decimal Qty { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxPercentage { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal otherTax { get; set; }
    public decimal extraTax { get; set; }
    public decimal extraTaxP { get; set; }
    public decimal furtherTaxP { get; set; }
    public decimal furtherTax { get; set; }
    public decimal fedPayable { get; set; }
    public decimal Amount { get; set; }
    public string saleType { get; set; } = "";
    public string sroItemSerialNo { get; set; } = "";
}
