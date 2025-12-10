using AuthModule.API.Dtos;
using System.ComponentModel.DataAnnotations;

namespace AuthModule.API.Models
{
    public class Company
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(250)]
        public string Name { get; set; } = null!;

        [MaxLength(100)]
        public string? ShortName { get; set; }

        [MaxLength(512)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? ContactPerson { get; set; }
        [MaxLength(100)]
        public string? CNIC { get; set; }
        [MaxLength(100)]
        public string? NTN { get; set; }
        [MaxLength(100)]
        public string? STRN { get; set; }
        [MaxLength(100)]
        public string? logoPath { get; set; }  
        [MaxLength(100)]
        public string? ContactEmail { get; set; }

        [MaxLength(50)]
        public string? ContactPhone { get; set; }
        public bool IsActive { get; set; } = true;
        public bool fbrActive { get; set; } = false;
        [MaxLength(100)]
        public string? pralToken { get; set; } = "";
        [MaxLength(100)]
        public string? Province { get; set; } = "";
        [MaxLength(100)]
        public string? fbrToken { get; set; } = "";
        public int invoiceCounter { get; set; } = 0;
        public int invoicePerPage { get; set; } = 0;
        public bool isGst { get; set; } = false;
        public double gstVal { get; set; } = 0;
        public bool isEd { get; set; } = false;
        public double edVal { get; set; } = 0;
        public bool isFed { get; set; } = false;
        public double fedVal { get; set; } = 0;
        public CompanyType CompanyType { get; set; }
    }
     
}
