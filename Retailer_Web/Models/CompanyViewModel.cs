namespace Retailer.Web.Models
{
    public class CompanyViewModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? ShortName { get; set; }
        public string? Address { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string? NTN { get; set; }
        public string? CNIC { get; set; } 
        public string? STRN { get; set; }
        public IFormFile? logo { get; set; }
        public string? logoPath { get; set; } 


        public bool fbrActive { get; set; }
        public string? Province { get; set; }
        public string? pralToken { get; set; } = "";
        public string? fbrToken { get; set; } = "";
        public int invoiceCounter { get; set; } = 0;
        public int invoicePerPage { get; set; } = 0;
    }
}
