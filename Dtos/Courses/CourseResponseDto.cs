namespace UNIOOP.App.Dtos.Courses
{
    public class CourseResponseDto
    {
        public int CourseID { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public short Credits { get; set; }
        public int UniversityID { get; set; }
        public string UniversityName { get; set; } = string.Empty;
        public int? TeacherID { get; set; }
        public string? TeacherName { get; set; } = string.Empty;
    }
}