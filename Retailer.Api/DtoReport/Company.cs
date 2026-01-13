namespace Retailer.Api.DtoReport
{
    public class CompanyDtoR
    {
        public string Name { get; set; } = string.Empty;    
        public string Address { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public byte[]? img { get; set; }   

    }
}
