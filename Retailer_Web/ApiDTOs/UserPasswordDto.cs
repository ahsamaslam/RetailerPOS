namespace Retailer.Web.ApiDTOs
{
    public class UserPasswordDto
    {
        public string userID { get; set; } = string.Empty;
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
