using System.ComponentModel.DataAnnotations;

namespace Retailer.POS.Api.Entities;
public class PurchaseMaster : BaseEntity
{
    public DateTime Date { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public int VendorID { get; set; }
    public int PurchaseType { get; set; }
    public Vendor? Vendor { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = "-";
    public int BranchId { get; set; }
    public Branch Branch { get; set; }
    public string? remarks { get; set; }
    public int Year { get; set; } = 1;
    public int Active { get; set; } = 1;
    public ICollection<PurchaseDetail> Details { get; set; } = new List<PurchaseDetail>();
}
