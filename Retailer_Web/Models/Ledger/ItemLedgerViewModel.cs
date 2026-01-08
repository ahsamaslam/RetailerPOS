using Retailer.POS.Web.ApiDTOs;
using Retailer.Web.ApiDTOs;

namespace Retailer.Web.Models.Ledger
{
    public class ItemLedgerViewModel
    {
        public int ItemId { get; set; } 
        public ItemDto Item { get; set; }
        public DateTime sdate { get; set; } = DateTime.Now; 
        public DateTime edate { get; set; } = DateTime.Now; 

    }
}
