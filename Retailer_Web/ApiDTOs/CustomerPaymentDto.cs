using Microsoft.AspNetCore.Mvc.ModelBinding;
using Retailer.Web.Models;

namespace Retailer.Web.ApiDTOs
{
    public class CustomerPaymentDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public CustomerDto? Customer { get; set; }
        public BankDto? Bank { get; set; }
        public string PaymentMethodName { get; set; }
        public int Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public PaymentMethod? PaymentMethod { get; set; }
        public int PaymentMethodId { get; set; }
        public int? bankId { get; set; }
        public string? bankName { get; set; }
        public decimal taxPercent { get; set; }
        public decimal totalAmount { get; set; }
        public decimal taxAmount { get; set; }
        public decimal whtPercent { get; set; }
        public decimal whtAmount { get; set; }
        public Guid companyId { get; set; }
        public Guid companyName { get; set; }
        public int status { get; set; }
        public int userCode { get; set; }
    }
}
