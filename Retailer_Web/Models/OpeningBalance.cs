using System.ComponentModel.DataAnnotations;

namespace Retailer.Web.Models
{
    public class OpeningBalanceViewModel
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public decimal OpeningQuantity { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public record CreateOpeningBalanceDto(
        [Required] int Year,
        [Required] int ProductId,
        [Required][Range(0, double.MaxValue)] decimal OpeningQuantity
    );

    public record UpdateOpeningBalanceDto(
        int Id,
        [Required] int Year,
        [Required] int ProductId,
        [Required][Range(0, double.MaxValue)] decimal OpeningQuantity
    );
}
