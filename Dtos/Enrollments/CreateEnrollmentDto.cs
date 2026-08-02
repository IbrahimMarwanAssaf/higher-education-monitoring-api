using System.ComponentModel.DataAnnotations;

namespace UNIOOP.App.Dtos.Enrollments
{
    public class CreateEnrollmentDto
    {
        [Range(1, int.MaxValue)]
        public int StudentID { get; set; }

        [Range(1, int.MaxValue)]
        public int CourseID { get; set; }
    }
}