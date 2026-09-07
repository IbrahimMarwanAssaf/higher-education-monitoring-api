using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UNIOOP.App.Models;

namespace UNIOOP.APP.Data.Configurations
{
    public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
    {
        public void Configure(EntityTypeBuilder<Teacher> builder)
        {
            builder.ToTable("Teacher", t =>
            {
                t.HasCheckConstraint(
                    "CK_Teachert_Salary",
                    "\"salary\" >= 0 ");
            });

            builder.Property(t => t.TeacherID).UseIdentityByDefaultColumn();
            builder.HasIndex(t => t.TeacherID).IsUnique().HasDatabaseName("UX_Teacher_TeacherID");

            builder.Property(t => t.Department).HasMaxLength(100).IsRequired();

            builder.Property(t => t.Salary).HasPrecision(12, 2).IsRequired();

            builder.Property(t => t.MinistryDegreeID).IsRequired(false);

            builder.Property(t => t.UniversityID).IsRequired();

            builder.HasOne(t => t.University)
                .WithMany(u => u.teachersCollection)
                .HasForeignKey(t => t.UniversityID)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Teacher_University");

            builder.HasIndex(t => t.UniversityID).HasDatabaseName("IX_Teacher_UniversityID");
        }
    }
}