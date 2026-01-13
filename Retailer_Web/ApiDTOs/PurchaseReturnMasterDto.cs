using Retailer.Web.ApiDTOs;
using System.ComponentModel.DataAnnotations;

namespace Retailer.POS.Web.ApiDTOs;
public class PurchaseReturnMasterDto
{
	public int Id { get; set; }
	public Guid CompanyId { get; set; }
	public DateTime Date { get; set; } = DateTime.UtcNow;
	public DateTime CreateDate { get; set; } = DateTime.UtcNow;
	public int VendorID { get; set; } 
	public string? VendorName { get; set; }
	public decimal SubTotal { get; set; }
	public decimal Discount { get; set; }
	public decimal TaxAmount { get; set; }
	public decimal TotalDiscount { get; set; }
	public decimal BalanceAmount { get; set; } 
	public decimal Total { get; set; }
	public Guid UserId { get; set; }
	public string UserName { get; set; } = "-";
	public int PurchaseType { get; set; } = 1;
	public int BranchId { get; set; } 
	public string? Remarks { get; set; }
	public int Year { get; set; } = 1;
	public int Active { get; set; } = 1;
	public List<PurchaseReturnDetailDto> Details { get; set; } = new();
}
public class PurchaseReturnDetailDto
{
	public int Id { get; set; }
	public int PurchaseReturnId { get; set; } 
	public int ItemId { get; set; } 
	public decimal Rate { get; set; }
	public string? ItemName { get; set; }
	public decimal Qty { get; set; }
	public decimal Discount { get; set; }
	public decimal Amount { get; set; }
	public decimal TaxAmount { get; set; }
	public decimal TaxPercentage { get; set; }
	public decimal subTotal { get; set; }
}
