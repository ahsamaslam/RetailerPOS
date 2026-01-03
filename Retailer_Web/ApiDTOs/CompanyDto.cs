namespace Retailer.Web.ApiDTOs
{
  
    public class CompanyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty; 
        public string ShortName { get; set; } = string.Empty; 
        public string Address { get; set; } = string.Empty; 
        public string ContactPerson { get; set; } = string.Empty; 
        public string NTN { get; set; } = string.Empty; 
        public string CNIC { get; set; } = string.Empty; 
        public string Province { get; set; } = string.Empty; 
        public string STRN { get; set; } = string.Empty; 
        public string? logoPath { get; set; }  
        public string ContactEmail { get; set; } = string.Empty; 
        public string ContactPhone { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public bool fbrActive { get; set; } = false;
        public string? pralToken { get; set; }
        public string? fbrToken { get; set; }
        public int invoiceCounter { get; set; } = 0;
        public int invoicePerPage { get; set; } = 0;
        public int Type { get; set; }
        public bool isGst { get; set; } = false;
        public double gstVal { get; set; } = 0;
        public bool isEd { get; set; } = false;
        public double edVal { get; set; } = 0;
        public bool isFed { get; set; } = false;
        public double fedVal { get; set; } = 0;
        public CompanyType CompanyType { get; set; }
    }
    public enum CompanyType
    {
        None,
        FBR,
        PRAL
    }
}
