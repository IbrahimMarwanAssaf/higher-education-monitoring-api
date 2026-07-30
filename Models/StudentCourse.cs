namespace UNIOOP.App.Models
{
    public partial class StudentCourse
    {
        public int CourseID { get; set; }
        public long StudentPersonnelID { get; set; }

        public StudentCourse() { }

        public StudentCourse(int courseID, long studentPersonnelID)
        {
            CourseID = courseID;
            StudentPersonnelID = studentPersonnelID;
        }
    }
}