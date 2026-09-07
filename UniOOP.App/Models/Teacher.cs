namespace UNIOOP.App.Models
{
    public partial class Teacher : Personnel
    {
        public int TeacherID { get; set; }
        public string Department { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public int? MinistryDegreeID { get; set; }
        public int UniversityID { get; set; }

        public University University { get; set; } = null!;
        public ICollection<Course> coursesCollection = [];

        public Teacher() { }

        public Teacher(
            string ssn, string fName, string lName,
            DateOnly dateOfBirth, string email, int teacherID,
            string department, decimal salary,
            int degreeID, int universityID) : base(ssn, fName, lName, dateOfBirth, email)
        {
            TeacherID = teacherID;
            Department = department;
            Salary = salary;
            MinistryDegreeID = degreeID;
            UniversityID = universityID;
        }

        public override void DisplayInfo()
        {
            Console.WriteLine($"Teacher ID: {TeacherID}");
            Console.WriteLine($"Name: {FName} {LName}");
            Console.WriteLine($"Date of Birth: {DateOfBirth}");
            Console.WriteLine($"Email: {Email}");
            Console.WriteLine($"Department: {Department}");
            Console.WriteLine($"Salary: {Salary}");
            Console.WriteLine($"Degree ID: {MinistryDegreeID}");
        }
    }
}