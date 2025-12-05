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
        public string? ContactEmail { get; set; }

        [MaxLength(50)]
        public string? ContactPhone { get; set; }

        // add more fields as needed (TaxId, IsActive, CreatedAt, etc.)
        public bool IsActive { get; set; } = true;
    }
}
