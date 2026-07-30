namespace UNIOOP.App.Models
{
    public partial class Course
    {
        public int CourseID { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public short Credits { get; set; }
        public int UniversityID { get; set; }
        public long? TeacherPersonnelID { get; set; }

        public Course() { }

        public Course(int courseID, string courseName, short credits, int universityID, long teacherPersonnelID)
        {
            CourseID = courseID;
            CourseName = courseName;
            Credits = credits;
            UniversityID = universityID;
            TeacherPersonnelID = teacherPersonnelID;
        }
    }
}