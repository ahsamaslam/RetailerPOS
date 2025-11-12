namespace Retailer.Api.DTOs
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public string? City { get; set; }
        public string? Province { get; set; }
        public string? Address { get; set; }
        public string? Mobile1 { get; set; }
        public string? Mobile2 { get; set; }
        public string? CNIC { get; set; }
    }
}
