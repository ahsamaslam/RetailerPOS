namespace Retailer.POS.Api.Entities;
public class PurchaseMaster : BaseEntity
{
    public DateTime Date { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public int SupplierID { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }
    public int LoginId { get; set; }
    public int BranchId { get; set; }
    public ICollection<PurchaseDetail> Details { get; set; } = new List<PurchaseDetail>();
}
