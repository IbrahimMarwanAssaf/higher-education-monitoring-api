using System.ComponentModel.DataAnnotations;

namespace UNIOOP.App.Dtos.Courses
{
    public class UpdateCourseDto
    {
        [Required]
        [MaxLength(150)]
        public string CourseName { get; set; } = string.Empty;

        [Range(1, 6)]
        public short Credits { get; set; }
        public int UniversityID { get; set; }
        public int? TeacherID { get; set; }
    }
}