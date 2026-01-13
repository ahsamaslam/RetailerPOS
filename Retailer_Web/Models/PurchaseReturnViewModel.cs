namespace Retailer.Web.Models
{
    public class PurchaseReturnViewModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal SubTotal { get; set; }
        public string vendorName { get; set; }
        public decimal Total { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal BalanceAmount { get; set; }
        public List<PurchaseReturnDetailViewModel> Details { get; set; } = new();
    }

    public class PurchaseReturnDetailViewModel
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public decimal Qty { get; set; }
    }

    

}
