namespace Retailer.POS.Web.DTOs;
public class PurchaseMasterDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public int VendorID { get; set; }  
    public decimal SubTotal { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }
    public int LoginId { get; set; } 
    public int BranchId { get; set; } 
    public List<PurchaseDetailDto> Details { get; set; } = new();
}
public class PurchaseDetailDto
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public decimal Qty { get; set; }
}
