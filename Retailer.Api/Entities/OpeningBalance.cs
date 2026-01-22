using Retailer.POS.Api.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Retailer.Api.Entities
{
    public class OpeningBalance : BaseEntity
    {
        [Key]
        public int OpeningBalanceId { get; set; }

        [Required]
        public int Year { get; set; }

        public int BranchId { get; set; }
        public Branch Branch { get; set; }

        /// <summary>
        /// Product name or SKU. If you have a Product table, replace with ProductId FK.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public int ProductID { get; set; } 

        [Required]
        public decimal OpeningQuantity { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [NotMapped]
        public decimal odlQuantity { get; set; } = 0;
    }
}
