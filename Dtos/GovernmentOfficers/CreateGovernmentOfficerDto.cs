using System.ComponentModel.DataAnnotations;

namespace UNIOOP.App.Dtos.GovernmentOfficers
{
    public class CreateGovernmentOfficerDto
    {
        [Required]
        [MaxLength(20)]
        public string SSN { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string FName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LName { get; set; } = string.Empty;

        public DateOnly DateOfBirth { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;
    }
}