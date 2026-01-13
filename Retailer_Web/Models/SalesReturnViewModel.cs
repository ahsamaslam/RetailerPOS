namespace Retailer.Web.Models
{
    public class SalesReturnViewModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal SubTotal { get; set; }
        public string customerName { get; set; }
        public decimal Total { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal BalanceAmount { get; set; }
        public List<SalesReturnDetailViewModel> Details { get; set; } = new();
    }

    public class SalesReturnDetailViewModel
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public decimal Qty { get; set; }
    }

    

}
