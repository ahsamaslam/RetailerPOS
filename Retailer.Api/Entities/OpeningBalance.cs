using Retailer.POS.Api.Entities;
using System.ComponentModel.DataAnnotations;

namespace Retailer.Api.Entities
{
    public class OpeningBalance : BaseEntity
    {
        [Key]
        public int OpeningBalanceId { get; set; }

        [Required]
        public int Year { get; set; }

        /// <summary>
        /// Product name or SKU. If you have a Product table, replace with ProductId FK.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public int ProductID { get; set; } 

        [Required]
        public decimal OpeningQuantity { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
