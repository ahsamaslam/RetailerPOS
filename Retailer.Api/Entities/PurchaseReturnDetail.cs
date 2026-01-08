namespace Retailer.POS.Api.Entities;
public class PurchaseReturnDetail : BaseEntity
{
    public int PurchaseReturnId { get; set; }
    public PurchaseReturnMaster? Purchase { get; set; }
    public int ItemId { get; set; }
    public Item? Item { get; set; }
    public decimal Rate { get; set; }
    public decimal Qty { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxPercentage { get; set; }
    public decimal TaxAmount { get; set; }
}
