namespace UNIOOP.App.Models
{
    public abstract partial class Personnel
    {
        public long PersonnelID { get; set; }
        public string SSN { get; set; } = string.Empty;
        public string FName { get; set; } = string.Empty;
        public string LName { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string Email { get; set; } = string.Empty;

        public abstract void DisplayInfo();

        protected Personnel() { }

        protected Personnel(
        string ssn,
        string fName,
        string lName,
        DateOnly dateOfBirth,
        string email)
        {
            SSN = ssn;
            FName = fName;
            LName = lName;
            DateOfBirth = dateOfBirth;
            Email = email;
        }
    }
}