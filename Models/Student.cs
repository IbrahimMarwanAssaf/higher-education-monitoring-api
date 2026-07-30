namespace UNIOOP.App.Models
{
    public partial class Student : Personnel
    {
        public int StudentID { get; set; }
        public string Major { get; set; } = string.Empty;
        public decimal GPA { get; set; }
        public int UniversityID { get; set; }

        public Student() { }

        public Student(
            string ssn,
            string firstName,
            string lastName,
            DateOnly dateOfBirth,
            string email,
            string major,
            decimal gpa,
            int universityID)
            : base(ssn, firstName, lastName, dateOfBirth, email)
        {
            Major = major;
            GPA = gpa;
            UniversityID = universityID;
        }

        public override void DisplayInfo()
        {
            Console.WriteLine($"Student ID: {StudentID}");
            Console.WriteLine($"Name: {FName} {LName}");
            Console.WriteLine($"Date of Birth: {DateOfBirth}");
            Console.WriteLine($"Email: {Email}");
            Console.WriteLine($"Major: {Major}");
            Console.WriteLine($"GPA: {GPA}");
        }
    }
}
