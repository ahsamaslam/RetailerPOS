using Microsoft.AspNetCore.Mvc.ModelBinding;
using Retailer.Web.ApiDTOs;

namespace Retailer.Web.Models
{
        public class CustomerPaymentViewModel
        {
            public int Id { get; set; }
            public int CustomerId { get; set; }
            public int Type { get; set; }
            public decimal Amount { get; set; }
            public DateTime CreatedDate { get; set; } =DateTime.Now;
            public DateTime PaymentDate { get; set; } =DateTime.Now;
           [BindNever] 
            public PaymentMethod? PaymentMethod { get; set; }
        public CustomerDto? Customer { get; set; }
        public BankDto? Bank { get; set; }
        public int PaymentMethodId { get; set; }
        [BindNever]
        public int? bankId { get; set; }
        [BindNever]
        public string? bankName { get; set; }
            public decimal taxPercent { get; set; } = 0;
            public decimal taxAmount { get; set; } = 0;
            public decimal totalAmount { get; set; } = 0;
            public decimal balance { get; set; } = 0;
            public decimal whtPercent { get; set; } = 0;
            public decimal whtAmount { get; set; } = 0;
            public Guid companyId { get; set; }
            public int userCode { get; set; }
            public int status { get; set; }
        }
        public class PaymentMethod
        {
            public int id { get; set; }
            public string Name { get; set; }
        }
}
