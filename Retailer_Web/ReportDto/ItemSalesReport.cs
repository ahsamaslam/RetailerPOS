namespace Retailer.Web.ReportDto
{
    public class ItemSalesReport
    {
        
            public int srno { get; set; }
            public int productCode { get; set; }
            public string productName { get; set; } = string.Empty;
            public int saleID { get; set; }
            public DateTime saleDate { get; set; }
            public string customerName { get; set; } = string.Empty;
            public decimal quantity { get; set; }
            public decimal unitPrice { get; set; }
            public decimal discount { get; set; }
        public decimal taxAmount { get; set; } = 0;
        
    }
}
