namespace Retailer.Api.DTOs
{
    public record CreateOpeningBalanceDto(int Year, int ProductID, decimal OpeningQuantity);
    public record UpdateOpeningBalanceDto(int Id, int Year, int ProductID, decimal OpeningQuantity);
    public class OpeningBalanceViewModel
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int ProductID { get; set; }  
        public decimal OpeningQuantity { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
