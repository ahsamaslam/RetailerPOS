using System.ComponentModel.DataAnnotations;

namespace Retailer.POS.Web.ApiDTOs
{
    public class UserDto
    {
        [Key]
        public string Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? picture { get; set; }
        public Guid? CompanyId { get; set; }
    }
}
