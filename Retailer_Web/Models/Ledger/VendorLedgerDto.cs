using Retailer.Web.ApiDTOs;

namespace Retailer.Web.Models.Ledger
{
    public class VendorLedgerDto
    {
        public int Id { get; set; }

        public int VendorId { get; set; }
        public VendorDto Vendor { get; set; }
        public string? Type { get; set; }
        public string? remarks { get; set; }
        public DateTime Date { get; set; }
        public string ReferenceType { get; set; } = null!; // Sale / Payment
        public int ReferenceId { get; set; }              // SalesMasterId / VendorPaymentId
        public int? yearId { get; set; }              // SalesMasterId / VendorPaymentId

        public decimal Debit { get; set; }   // Sale amount
        public decimal Credit { get; set; }  // Payment amount

        public decimal Balance { get; set; } // Running balance (optional but useful)

        public Guid CompanyId { get; set; }
    }
}
