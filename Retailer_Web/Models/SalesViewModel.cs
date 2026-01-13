namespace Retailer.Web.Models
{
    public class SalesViewModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal SubTotal { get; set; }
        public string customerName { get; set; }
        public decimal Total { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal BalanceAmount { get; set; }
        public List<SalesDetailViewModel> Details { get; set; } = new();
    }

    public class SalesDetailViewModel
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public decimal Qty { get; set; }
    }

    

}
