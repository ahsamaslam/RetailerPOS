namespace Retailer.Web.Models
{
    public class PurchaseViewModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public List<SalesDetailViewModel> Details { get; set; } = new();
    }

    public class PurchaseDetailViewModel
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public decimal Qty { get; set; }
    }

    

}
