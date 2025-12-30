using Retailer.POS.Api.Entities;

namespace Retailer.Api.Entities
{
    public class VendorPayment
    {

        public int Id { get; set; }
        public int VendorId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } 
        public Vendor? Vendor { get; set; }  
        public Banks? Bank { get; set; }  
        public int? BankId { get; set; }  
        public PaymentMethod? PaymentMethod { get; set; }  
        public int PaymentMethodId { get; set; }
        public string? bankName { get; set; } = "";
        public decimal taxPercent { get; set; } = 0;
        public decimal taxAmount { get; set; } = 0;
        public decimal whtPercent { get; set; } = 0;
        public decimal whtAmount { get; set; } = 0;
        public Guid companyId { get; set; }
        public int status { get; set; } = 1;
        public int userCode { get; set; }
        public string? remarks { get; set; }
    } 
}
