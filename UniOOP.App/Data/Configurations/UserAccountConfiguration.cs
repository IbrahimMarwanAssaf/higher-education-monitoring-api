using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UNIOOP.App.Models;

namespace UNIOOP.APP.Data.Configurations
{
    public class UserAccountConfiguration : IEntityTypeConfiguration<UserAccount>
    {
        public void Configure(EntityTypeBuilder<UserAccount> builder)
        {
            builder.ToTable("UserAccount");
            builder.HasKey(u => u.UserAccountID);
            builder.Property(u => u.UserAccountID).UseIdentityByDefaultColumn();
            builder.Property(u => u.PasswordHash).IsRequired();

            builder.HasOne(u => u.Personnel)
                .WithOne()
                .HasForeignKey<UserAccount>(u => u.PersonnelID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(u => u.PersonnelID).IsUnique();
        }
    }
}