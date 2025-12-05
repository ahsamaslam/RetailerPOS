using System.ComponentModel.DataAnnotations;

namespace AuthModule.API.Dtos
{
    public class CompanyCreateDto
    {
        [Required]
        [MaxLength(250)]
        public string Name { get; set; } = null!;

        [MaxLength(100)]
        public string? ShortName { get; set; }

        [MaxLength(512)]
        public string? Address { get; set; }

        [MaxLength(100)]
        [EmailAddress]
        public string? ContactEmail { get; set; }

        [MaxLength(50)]
        public string? ContactPhone { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
