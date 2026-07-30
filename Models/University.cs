namespace UNIOOP.App.Models
{
    public partial class University
    {
        public int UniversityID { get; set; }
        public string UniversityName { get; set; } = string.Empty;

        public University() { }

        public University(string universityName)
        {
            UniversityName = universityName;
        }
    }
}