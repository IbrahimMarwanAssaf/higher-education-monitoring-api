namespace UNIOOP.App.Models
{
    public class UserAccount
    {
        public int UserAccountID { get; set; }
        public string PasswordHash { get; set; } = null!;
        public string Role { get; set; } = null!;
        public long PersonnelID { get; set; }
        public Personnel Personnel { get; set; } = null!;
    }
}