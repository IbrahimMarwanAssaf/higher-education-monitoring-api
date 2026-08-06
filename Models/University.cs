namespace UNIOOP.App.Models
{
    public partial class University
    {
        public int UniversityID { get; set; }
        public string UniversityName { get; set; } = string.Empty;

        public ICollection<Student> studentsCollection = [];
        public ICollection<Teacher> teachersCollection = [];
        public ICollection<Course> coursesCollection = [];

        public University() { }

        public University(string universityName)
        {
            UniversityName = universityName;
        }
    }
}