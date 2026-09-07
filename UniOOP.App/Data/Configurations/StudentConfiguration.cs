using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UNIOOP.App.Models;

namespace UNIOOP.APP.Data.Configurations
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.ToTable("Student", t =>
           {
               t.HasCheckConstraint(
                   "CK_Student_GPA",
                   "\"gpa\" >= 0.00 AND \"gpa\" <= 4.00");
           });

            builder.Property(s => s.StudentID).UseIdentityByDefaultColumn();
            builder.HasIndex(s => s.StudentID).IsUnique().HasDatabaseName("UX_Student_StudentID");

            builder.Property(s => s.Major).HasMaxLength(100).IsRequired();

            builder.Property(s => s.GPA).HasPrecision(3, 2).IsRequired();

            builder.HasOne(s => s.University)
                .WithMany(u => u.studentsCollection)
                .HasForeignKey(s => s.UniversityID)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Student_University");

            builder.HasIndex(s => s.UniversityID).HasDatabaseName("IX_Student_UniversityID");
        }
    }
}