namespace Retailer.Api.DTOs
{
    public class LoginDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public List<string> Roles { get; set; }
    }
}
