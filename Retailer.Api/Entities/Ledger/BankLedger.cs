using Retailer.POS.Api.Entities;

namespace Retailer.Api.Entities.Ledger
{
    public class BankLedger
    {
        public int Id { get; set; }

        public int BankId { get; set; }
        public Banks Bank { get; set; } = null!;
        public string? Type { get; set; }
        public string? remarks { get; set; }
        public DateTime Date { get; set; }
        public string ReferenceType { get; set; } = null!; // "CustomerPayment", "VendorPayment", "Adjustment"
        public int ReferenceId { get; set; } // Payment Id or Adjustment Id
        public int? yearId { get; set; }
        public decimal Debit { get; set; }  // Money in
        public decimal Credit { get; set; } // Money out

        public decimal Balance { get; set; } // Running balance
        public Guid CompanyId { get; set; }
    }
}
