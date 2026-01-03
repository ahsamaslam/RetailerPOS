using Retailer.Api.Entities;

namespace Retailer.Api.DTOs
{
    public class VendorPaymentDto
    {
        public int Id { get; set; }
        public int VendorId { get; set; }
        public int Type { get; set; }
        public decimal Amount { get; set; } = 0;
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public PaymentMethod? PaymentMethod { get; set; }
        public int PaymentMethodId { get; set; }
        public int? bankId { get; set; }
        public string? bankName { get; set; }
        public decimal taxPercent { get; set; }
        public decimal taxAmount { get; set; }
        public decimal totalAmount { get; set; }
        public decimal whtPercent { get; set; }
        public decimal whtAmount { get; set; }
        public Guid companyId { get; set; }
        public Guid companyName { get; set; } 
        public int status { get; set; }
        public int userCode { get; set; }= 0;
        public string? remarks { get; set; }
    }
}
