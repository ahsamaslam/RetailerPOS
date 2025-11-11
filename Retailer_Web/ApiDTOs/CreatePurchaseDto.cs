namespace Retailer.POS.Web.DTOs;
public class CreatePurchaseDto
{
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public int SupplierID { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }
    public int LoginId { get; set; }
    public int BranchId { get; set; }
    public List<CreatePurchaseDetailDto> Details { get; set; } = new();
}
public class CreatePurchaseDetailDto
{
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public decimal Qty { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxPercentage { get; set; }
}
