using Retailer.POS.Api.Entities;

namespace Retailer.Api.Entities.Views
{
    public class vwStockLedger : BaseEntity
    {
        public int OpeningBalanceId { get; set; }  
        public int Id { get; set; }  
        public int ProductID { get; set; }  
        public decimal Rate { get; set; }  
        public decimal Qty { get; set; }
        public decimal Discount { get; set; }
        public decimal TaxPercentage { get; set; }
        public DateTime CreatedAt { get; set; }

        public int Year { get; set; }
    }
}
