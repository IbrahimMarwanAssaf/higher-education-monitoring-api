namespace UNIOOP.App.Dtos.Teachers
{
    public class TeacherResponseDto
    {
        public int TeacherID { get; set; }
        public string FName { get; set; } = string.Empty;
        public string LName { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public int? MinistryDegreeID { get; set; }
        public int UniversityID { get; set; }
        public string UniversityName { get; set; } = string.Empty;
    }
}