using Microsoft.AspNetCore.Mvc.Rendering;

namespace Retailer.Web.Dtos
{
	public class ItemSelectListItem: SelectListItem
	{
		public decimal rate { get; set; }
		public decimal cost { get; set; }
	}
}
