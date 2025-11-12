namespace Retailer.Web.ApiDTOs
{
    public class BranchDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? ContactPerson { get; set; }
        public string? MobileNo { get; set; }
        public string? BillHeading1 { get; set; }
        public string? BillHeading2 { get; set; }
        public string? BillMobileNo { get; set; }
    }
}
