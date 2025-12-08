using Retailer.Web.ApiDTOs;

namespace Retailer.Web.Models
{
	public class LayoutUserInfo
	{
		public string UserName { get; set; } = "";
		public string AvatarUrl { get; set; } = "/assets/img/user2-160x160.jpg";
		public bool IsAdmin { get; set; } = false;

    }
}
