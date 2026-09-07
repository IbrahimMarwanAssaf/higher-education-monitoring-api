using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UNIOOP.App.Models;

namespace UNIOOP.APP.Data.Configurations
{
    public class PersonnelConfiguration : IEntityTypeConfiguration<Personnel>
    {
        public void Configure(EntityTypeBuilder<Personnel> builder)
        {
            builder.UseTptMappingStrategy();
            builder.ToTable("Personnel");

            builder.HasKey(p => p.PersonnelID);

            builder.Property(p => p.PersonnelID).UseIdentityByDefaultColumn();

            builder.Property(p => p.SSN).HasMaxLength(20).IsRequired();
            builder.HasIndex(p => p.SSN).IsUnique().HasDatabaseName("UX_Personnel_SSN");

            builder.Property(p => p.FName).HasMaxLength(50).IsRequired();
            builder.Property(p => p.LName).HasMaxLength(50).IsRequired();

            builder.Property(p => p.DateOfBirth).IsRequired();

            builder.Property(p => p.Email).HasMaxLength(150).IsRequired();
            builder.HasIndex(p => p.Email).IsUnique().HasDatabaseName("UX_Personnel_Email");
        }
    }
}