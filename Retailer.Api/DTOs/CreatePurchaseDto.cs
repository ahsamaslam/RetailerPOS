using Retailer.POS.Api.Entities;

namespace Retailer.POS.Api.DTOs;
public class CreatePurchaseDto
{ 
    public DateTime Date { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public int VendorID { get; set; }
    public string? VendorName { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }
    public int LoginId { get; set; }
    public string? LoginName { get; set; }
    public int BranchId { get; set; }
    public string? BranchName { get; set; }
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
