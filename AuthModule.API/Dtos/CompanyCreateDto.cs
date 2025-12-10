using System.ComponentModel.DataAnnotations;

namespace AuthModule.API.Dtos
{
    public class CompanyCreateDto
    {
        [Required]
        [MaxLength(250)]
        public string Name { get; set; } = null!;

        [MaxLength(100)]
        public string? CNIC { get; set; }
        [MaxLength(100)]
        public string? NTN { get; set; }
        [MaxLength(100)]
        public string? ShortName { get; set; }

        [MaxLength(512)]
        public string? Address { get; set; }

        [MaxLength(100)]
        [EmailAddress]
        public string? ContactEmail { get; set; }

        [MaxLength(50)]
        public string? ContactPhone { get; set; }

        public bool IsActive { get; set; }
        public bool fbrActive { get; set; }
        public string? pralToken { get; set; }
        public string? logoPath { get; set; }
        public string? fbrToken { get; set; }
        public int invoiceCounter { get; set; } = 0;
        public int invoicePerPage { get; set; } = 0;
        public string? Province { get; set; } = "";
        public CompanyType CompanyType { get; set; }
        public bool isGst { get; set; } = false;
        public double gstVal { get; set; } = 0;
        public bool isEd { get; set; } = false;
        public double edVal { get; set; } = 0;
        public bool isFed { get; set; } = false;
        public double fedVal { get; set; } = 0; 
    }
     
}
