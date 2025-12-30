using Retailer.Api.DTOs;
using Retailer.POS.Api.Entities;

namespace Retailer.Api.Entities.Ledger
{
    public class CustomerLedger
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public string? Type { get; set; }
        public string? remarks { get; set; }
      public DateTime Date { get; set; } 
        public string ReferenceType { get; set; } = null!; // Sale / Payment
        public int ReferenceId { get; set; }              // SalesMasterId / CustomerPaymentId
        public int? yearId { get; set; }              // SalesMasterId / CustomerPaymentId

        public decimal Debit { get; set; }   // Sale amount
        public decimal Credit { get; set; }  // Payment amount

        public decimal Balance { get; set; } // Running balance (optional but useful)

        public Guid CompanyId { get; set; }
    }
}
