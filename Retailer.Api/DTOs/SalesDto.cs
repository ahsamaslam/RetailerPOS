namespace Retailer.Api.DTOs
{
    public class SalesMasterDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int LoginId { get; set; }
        public int BranchId { get; set; }
        public string? CustomerName { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal BalanceAmount { get; set; }
        public string? CustomerCode { get; set; }
        public List<SalesDetailDto> Details { get; set; } = new List<SalesDetailDto>();
    }

    public class SalesDetailDto
    {
        public int Id { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public decimal Qty { get; set; }
        public decimal Discount { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Amount { get; set; }
    }
}
