using Retailer.Web.ApiDTOs;
using System.ComponentModel.DataAnnotations.Schema;

namespace Retailer.POS.Web.ApiDTOs
{
    public class SalesMasterDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public DateTime CreateDate { get; set; } = DateTime.UtcNow; 
        public Guid UserId { get; set; }
        public int BranchId { get; set; }
        public string? SaleType { get; set; } 
        public decimal SubTotal { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal BalanceAmount { get; set; }
        public int? CustomerCode { get; set; }
        public int CustomerID { get; set; }  
        public int CategoryId { get; set; }  
        public List<SalesDetailDto> Details { get; set; } = new();
        [NotMapped]
        public string? CustomerName { get; set; }
    }

    public class SalesDetailDto
    {
        public int Id { get; set; }
        public int SalesMasterId { get; set; } 
        public int ItemCode { get; set; }    // <- must be string
        public int CategoryId { get; set; }    // <- must be string
        public int ItemId { get; set; }    // <- must be string
        public string ItemName { get; set; } = string.Empty;   // <- must be string
        public string HsCode { get; set; } = string.Empty;   // <- must be string
        public string UOM { get; set; } = string.Empty;   // <- must be string
        public string SaleType { get; set; } = string.Empty;   // <- must be string
        public decimal Rate { get; set; }
        public decimal Qty { get; set; }
        public decimal subTotal { get; set; }
        public decimal totalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal TaxPercentage { get; set; }
		public decimal extraTax { get; set; }
		public decimal extraTaxP { get; set; }
		public decimal furtherTaxP { get; set; }
		public decimal furtherTax { get; set; }
		public decimal TaxAmount { get; set; }
        public decimal Amount { get; set; }
        [NotMapped]
        public decimal Stock { get; set; }
    }
}
