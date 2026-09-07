using System.ComponentModel.DataAnnotations;

namespace UNIOOP.App.Dtos.Students
{
    public class CreateStudentDto
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
        [MaxLength(100)]
        public string Major { get; set; } = string.Empty;

        [Range(0, 4)]
        public decimal GPA { get; set; }

        public int UniversityID { get; set; }

    }
}