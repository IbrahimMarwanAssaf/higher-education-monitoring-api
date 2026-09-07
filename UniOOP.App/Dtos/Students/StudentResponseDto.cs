namespace UNIOOP.App.Dtos.Students
{
    public class StudentResponseDto
    {
        public int StudentID { get; set; }
        public string FName { get; set; } = string.Empty;
        public string LName { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Major { get; set; } = string.Empty;
        public decimal GPA { get; set; }
        public int UniversityID { get; set; }
        public string UniversityName { get; set; } = string.Empty;
    }
}