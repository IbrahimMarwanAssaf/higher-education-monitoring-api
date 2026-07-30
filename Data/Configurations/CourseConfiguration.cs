using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UNIOOP.App.Models;

namespace UNIOOP.APP.Data.Configurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.ToTable("Course", t =>
           {
               t.HasCheckConstraint("CK_Course_Credits", "credits BETWEEN 1 AND 6");
           });

            builder.HasKey(c => c.CourseID).HasName("PK_Course");

            builder.Property(c => c.CourseID).UseIdentityByDefaultColumn();

            builder.Property(c => c.CourseName).HasMaxLength(150).IsRequired();

            builder.Property(c => c.Credits).IsRequired();

            builder.Property(c => c.UniversityID).IsRequired();
            builder.Property(c => c.TeacherPersonnelID).IsRequired(false);

            builder.HasIndex(c => new
            {
                c.UniversityID,
                c.CourseName
            }).IsUnique().HasDatabaseName("UX_Course_UniversityID_CourseName");

            builder.HasOne<University>()
                .WithMany()
                .HasForeignKey(c => c.UniversityID)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Course_University");

            builder.HasOne<Teacher>()
                .WithMany()
                .HasForeignKey(c => c.TeacherPersonnelID)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Course_Teacher");

            builder.HasIndex(c => c.UniversityID)
                .HasDatabaseName("IX_Course_UniversityID");

            builder.HasIndex(c => c.TeacherPersonnelID)
                .HasDatabaseName("IX_Course_TeacherPersonnelID");
        }
    }
}