namespace Retailer.POS.Web.ApiDTOs;
public class CreatePurchaseReturnDto
{
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public int VendorID { get; set; }
    public int PurchaseType { get; set; } = 1;
    public decimal SubTotal { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }
    public int LoginId { get; set; }
    public int BranchId { get; set; }
    public int Active { get; set; } = 1;
    public int Year { get; set; } = 1;
    public List<CreatePurchaseReturnDetailDto> Details { get; set; } = new();
}
public class CreatePurchaseReturnDetailDto
{
    public int Id { get; set; }
    public int PurchaseReturnId { get; set; }
    public int ItemId { get; set; }
    public string? ItemName{ get; set; }
    public decimal Rate { get; set; }
    public decimal Qty { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TaxPercentage { get; set; }
} 