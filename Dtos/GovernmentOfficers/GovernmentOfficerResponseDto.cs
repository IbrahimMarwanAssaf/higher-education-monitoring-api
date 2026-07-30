namespace UNIOOP.App.Dtos.GovernmentOfficers
{
    public class GovernmentOfficerResponseDto
    {
        public int OfficerID { get; set; }
        public string SSN { get; set; } = string.Empty;
        public string FName { get; set; } = string.Empty;
        public string LName { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}