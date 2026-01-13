using Retailer.POS.Api.Entities;

namespace Retailer.POS.Api.DTOs;
public class CreatePurchaseReturnDto
{ 
    public int Id { get; set; } 
    public DateTime Date { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public int VendorID { get; set; }
    public int PurchaseType { get; set; }
    public string? VendorName { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }
    public Guid UserId { get; set; }
    public string? UserName { get; set; }
    public string? remarks { get; set; }
    public int BranchId { get; set; }
    public string? BranchName { get; set; }
    public int? year { get; set; } = 1;
    public int? Active { get; set; } = 1;
    public List<CreatePurchaseReturnDetailDto> Details { get; set; } = new();
}

public class CreatePurchaseReturnDetailDto
{
    public int Id { get; set; }
    public int PurchaseReturnId { get; set; }
    public int ItemId { get; set; }
    public string? ItemName { get; set; } 
    public decimal Rate { get; set; }
    public decimal Qty { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxPercentage { get; set; }
    public decimal TaxAmount { get; set; }
}
