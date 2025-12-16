using Retailer.Web.ApiDTOs;

namespace Retailer.Web.Models
{
    public class CustomerViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? CNIC { get; set; }
        public string? NTN { get; set; }
        public string? STRN { get; set; }
        public string? Mobile { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; } 
        public string? Province { get; set; }
        public DateTime? openDate { get; set; }
        public bool Register { get; set; } = false;
        public int? CityId { get; set; }
        public double? openingBalance { get; set; } = 0;
        public PaymentType? PaymentType { get; set; }
        public CitiesDto? City { get; set; }
    }
    public enum PaymentType
    {
        Cash,
        Credit
    }
}
