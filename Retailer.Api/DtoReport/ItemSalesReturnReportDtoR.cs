namespace Retailer.Api.DtoReport
{
    public class ItemSalesReturnReportDtoR
    {
        public int srno { get; set; }   
        public int productCode { get; set; }    
        public string productName { get; set; } = string.Empty; 
        public int salesReturnID { get; set; }
        public DateTime salesReturnDate { get; set; }  
        public string customerName { get; set; } = string.Empty;  
        public decimal quantity { get; set; }
        public decimal unitPrice { get; set; }  
        public decimal discount { get; set; }   
        public decimal taxAmount { get; set; }   
        public decimal subTotal { get; set; }   
    }
}
