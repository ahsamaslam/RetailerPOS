using System.ComponentModel.DataAnnotations;

namespace Retailer.POS.Web.ApiDTOs;
public class PurchaseMasterDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public int VendorID { get; set; }  
    public string? PurchaseType { get; set; }  
    public decimal SubTotal { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal BalanceAmount { get; set; }
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
    public decimal subTotal { get; set; }
    [Range(0, 100, ErrorMessage = "Tax % must be between 0 and 100.")]
    public decimal TaxPercentage { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Amount { get; set; }
}
