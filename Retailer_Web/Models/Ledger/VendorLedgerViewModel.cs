using Retailer.Web.ApiDTOs;

namespace Retailer.Web.Models.Ledger
{
    public class VendorLedgerViewModel
    {
        public int VendorId { get; set; } 
        public VendorDto Vendor { get; set; }
        public DateTime sdate { get; set; } = DateTime.Now; 
        public DateTime edate { get; set; } = DateTime.Now; 

    }
}
