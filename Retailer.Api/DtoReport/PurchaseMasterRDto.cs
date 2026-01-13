namespace Retailer.Api.DtoReport
{
    public class PurchaseMasterRDto
    {
        public int Id { get; set; }  
        public DateTime Date { get; set; }
        public string VendorName { get; set; } = string.Empty; 
        public double SubTotal { get; set; }
        public double TotalDiscount { get; set; }
        public double TaxAmount { get; set; }
        public double Total { get; set; } 
    }
}
