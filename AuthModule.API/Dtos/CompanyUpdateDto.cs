using System.ComponentModel.DataAnnotations;

namespace AuthModule.API.Dtos
{
    public class CompanyUpdateDto
    {
        [Required]
        [MaxLength(250)]
        public string Name { get; set; } = null!;

        [MaxLength(100)]
        public string? ShortName { get; set; }

        [MaxLength(512)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? CNIC { get; set; }
        [MaxLength(100)]
        public string? NTN { get; set; } 
        [MaxLength(100)]
        public string? STRN { get; set; }
        [MaxLength(100)]
        public string? logoPath { get; set; }
        [EmailAddress]
        public string? ContactEmail { get; set; }

        [MaxLength(50)]
        public string? ContactPhone { get; set; }

        public bool IsActive { get; set; } = true;

        public bool fbrActive { get; set; } = true;
        [MaxLength(250)]
        public string? pralToken { get; set; }
        [MaxLength(250)]
        public string? fbrToken { get; set; }
        public int invoiceCounter { get; set; } = 0;
        public int invoicePerPage { get; set; } = 0;
        public string? Province { get; set; } = "";
    }
}
