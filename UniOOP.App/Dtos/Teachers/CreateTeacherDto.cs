using System.ComponentModel.DataAnnotations;

namespace UNIOOP.App.Dtos.Teachers
{
    public class CreateTeacherDto
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
        public string Department { get; set; } = string.Empty;

        [Range(typeof(decimal), "0", "9999999999.99")]
        public decimal Salary { get; set; }
        public int? MinistryDegreeID { get; set; }

        [Required]
        public int UniversityID { get; set; }
    }
}