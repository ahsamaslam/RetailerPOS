namespace Retailer.Api.DTOs
{
    public record CreateOpeningBalanceDto(int Year, string Product, decimal OpeningQuantity);
    public record UpdateOpeningBalanceDto(int Id, int Year, string Product, decimal OpeningQuantity);
    public class OpeningBalanceViewModel
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public string Product { get; set; } = "";
        public decimal OpeningQuantity { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
