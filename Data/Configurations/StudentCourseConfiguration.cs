using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UNIOOP.App.Models;

namespace UNIOOP.APP.Data.Configurations
{
    public class StudentCourseConfiguration : IEntityTypeConfiguration<StudentCourse>
    {
        public void Configure(EntityTypeBuilder<StudentCourse> builder)
        {
            builder.ToTable("StudentCourse");

            builder.HasKey(sc => new
            {
                sc.StudentPersonnelID,
                sc.CourseID
            }).HasName("PK_StudentCourse");

            builder.Property(sc => sc.StudentPersonnelID).IsRequired();

            builder.Property(sc => sc.CourseID).IsRequired();

            builder.HasOne<Student>()
                .WithMany()
                .HasForeignKey(sc => sc.StudentPersonnelID)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_StudentCourse_Student");

            builder.HasOne<Course>()
                .WithMany()
                .HasForeignKey(sc => sc.CourseID)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_StudentCourse_Course");

            builder.HasIndex(sc => sc.CourseID).HasDatabaseName("IX_StudentCourse_CourseID");
        }
    }
}