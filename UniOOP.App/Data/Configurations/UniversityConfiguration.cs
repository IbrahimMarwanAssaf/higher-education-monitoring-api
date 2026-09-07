using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UNIOOP.App.Models;

namespace UNIOOP.APP.Data.Configurations
{
    public class UniversityConfiguration : IEntityTypeConfiguration<University>
    {
        public void Configure(EntityTypeBuilder<University> builder)
        {
            builder.ToTable("University");

            builder.HasKey(u => u.UniversityID).HasName("PK_University");
            builder.Property(u => u.UniversityID).UseIdentityByDefaultColumn();

            builder.Property(u => u.UniversityName).HasMaxLength(150).IsRequired();

            builder.HasIndex(u => u.UniversityName).IsUnique().HasDatabaseName("UX_University_UniversityName");
        }
    }
}