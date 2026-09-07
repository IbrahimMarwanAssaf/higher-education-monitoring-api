namespace UNIOOP.App.Models
{
    public partial class GovernmentOfficer : Personnel
    {
        public int OfficerID { get; set; }

        public GovernmentOfficer() { }

        public GovernmentOfficer(string ssn, string fName, string lName, DateOnly dateOfBirth,
            string email, int officerID)
                : base(ssn, fName, lName, dateOfBirth, email)
        {
            OfficerID = officerID;
        }

        public override void DisplayInfo()
        {
            Console.WriteLine($"Officer ID: {OfficerID}");
            Console.WriteLine($"Name: {FName} {LName}");
            Console.WriteLine($"Date of Birth: {DateOfBirth}");
            Console.WriteLine($"Email: {Email}");
        }
    }
}
