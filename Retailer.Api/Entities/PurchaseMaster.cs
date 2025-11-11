namespace Retailer.POS.Api.Entities;
public class PurchaseMaster : BaseEntity
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public int VendorID { get; set; }
    public Vendor? Vendor { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }
    public int LoginId { get; set; }
    public Login Login { get; set; }
    public int BranchId { get; set; }
    public Branch Branch { get; set; }
    public ICollection<PurchaseDetail> Details { get; set; } = new List<PurchaseDetail>();
}
