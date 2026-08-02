namespace UNIOOP.App.Dtos.Enrollments
{
    public class EnrollmentResponseDto
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; } = string.Empty;

        public int CourseID { get; set; }
        public string CourseName { get; set; } = string.Empty;

        public int UniversityID { get; set; }
        public string UniversityName { get; set; } = string.Empty;
    }
}