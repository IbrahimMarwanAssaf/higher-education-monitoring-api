using UNIOOP.App.Interfaces;

namespace UNIOOP.App.Models
{
    public partial class GovernmentOfficer : Personnel, IReportGenerator
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

        public Report GenerateReport(ReportType type)
        {
            string content;

            if (type == ReportType.Compliance)
            {
                content =
                    "COMPLIANCE REPORT\n" +
                    "Purpose: Review university compliance with ministry requirements.\n" +
                    "Scope: Universities, courses, teachers, students, and teacher qualification verification.\n" +
                    "Result: The officer should use university audit and teacher verification functions to complete the compliance review.\n" +
                    "Status: Compliance report generated successfully.";
            }
            else if (type == ReportType.Salary)
            {
                content =
                    "SALARY REPORT\n" +
                    "Purpose: Review teacher salary records.\n" +
                    "Scope: Teacher ID, department, salary amount, and related salary audit information.\n" +
                    "Result: The officer should use teacher salary audit to check individual salary records.\n" +
                    "Status: Salary report generated successfully.";
            }
            else
            {
                content = "Unknown report type.";
            }

            return new Report(Random.Shared.Next(1000, 9999), content, DateTime.Now);
        }
    }
}
