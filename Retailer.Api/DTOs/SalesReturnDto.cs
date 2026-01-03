namespace Retailer.Api.DTOs
{
    public class SalesReturnMasterDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int BranchId { get; set; }
        public string? CustomerName { get; set; }
        public string? SaleType { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal BalanceAmount { get; set; }
        public int? CustomerCode { get; set; }
        public List<SalesReturnDetailDto> Details { get; set; } = new List<SalesReturnDetailDto>();
    }

    public class SalesReturnDetailDto
    {
        public int Id { get; set; }
        public int ItemCode { get; set; } 
        public string ItemName { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public decimal Qty { get; set; }
        public decimal Discount { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Amount { get; set; }
		public decimal extraTaxP { get; set; }
		public decimal furtherTaxP { get; set; }
		public decimal extraTax { get; set; }
		public decimal furtherTax { get; set; }
	}
}
