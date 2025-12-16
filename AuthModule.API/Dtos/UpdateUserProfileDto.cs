using System.ComponentModel.DataAnnotations;

namespace AuthModule.API.Dtos
{
    public class UpdateUserProfileDto
    {
        [Required]
        public string UserId { get; set; } = default!;

        [Required]
        public string UserName { get; set; } = default!;

        [EmailAddress]
        public string? Email { get; set; }

        public IFormFile? Picture { get; set; }
    }
}
