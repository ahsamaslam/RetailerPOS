namespace Retailer.Web.Models
{
    public class CreateSalesViewModel
    {
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public int LoginId { get; set; }
        public int BranchId { get; set; }
        public string? CustomerName { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal BalanceAmount { get; set; }
        public string? CustomerCode { get; set; }
        public List<SalesDetailViewModel> Details { get; set; } = new();
    }

    public class SalesDetailViewModel
    {
        public string ItemCode { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public decimal Qty { get; set; }
        public decimal Discount { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Amount { get; set; }
    }

    public class SalesViewModel
    {
        public int Id { get; set; }
        public CreateSalesViewModel? Master { get; set; }
        public List<SalesDetailViewModel> Details { get; set; } = new();
    }

}
