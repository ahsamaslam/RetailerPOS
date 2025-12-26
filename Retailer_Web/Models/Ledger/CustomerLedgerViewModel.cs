using Retailer.Web.ApiDTOs;

namespace Retailer.Web.Models.Ledger
{
    public class CustomerLedgerViewModel
    {
        public int CustomerId { get; set; } 
        public CustomerDto Customer { get; set; }
        public DateTime sdate { get; set; } = DateTime.Now; 
        public DateTime edate { get; set; } = DateTime.Now; 

    }
}
