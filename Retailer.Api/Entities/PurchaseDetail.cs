namespace Retailer.POS.Api.Entities;
public class PurchaseDetail : BaseEntity
{
    public int PurchaseId { get; set; }
    public PurchaseMaster? Purchase { get; set; }
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public decimal Qty { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxPercentage { get; set; }
}
