using Retailer.POS.Api.Entities;

namespace Retailer.POS.Api.DTOs;
public class PurchaseMasterDto
{
	public int Id { get; set; }
	public Guid CompanyId { get; set; }
	public DateTime Date { get; set; }
	public DateTime CreateDate { get; set; } = DateTime.UtcNow;
	public int VendorID { get; set; } 
	public decimal TotalDiscount { get; set; }
	public decimal BalanceAmount { get; set; }
	public decimal SubTotal { get; set; }
	public decimal Discount { get; set; }
	public decimal TaxAmount { get; set; }
	public decimal Total { get; set; }
	public Guid UserId { get; set; }
	public int PurchaseType { get; set; } = 1;
	public string UserName { get; set; } = "-";
	public int BranchId { get; set; } 
	public string? Remarks { get; set; }
	public int Year { get; set; } = 1; 
	public int Active { get; set; } = 1; 
	public List<PurchaseDetailDto> Details { get; set; } = new();
}

public class PurchaseDetailDto
{
	public int Id { get; set; }
	public int PurchaseId { get; set; } 
	public int ItemId { get; set; } public string? ItemName { get; set; }
	public decimal Rate { get; set; }
	public decimal Qty { get; set; }
	public decimal Discount { get; set; }
	public decimal TaxAmount { get; set; }
	public decimal TaxPercentage { get; set; }
	public decimal Amount { get; set; }
	public decimal subTotal { get; set; }
}
