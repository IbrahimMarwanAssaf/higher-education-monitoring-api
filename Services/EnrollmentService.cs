using Microsoft.EntityFrameworkCore;
using UNIOOP.App.Data;
using UNIOOP.App.Dtos.Enrollments;
using UNIOOP.App.Models;
using UNIOOP.App.Services.Interfaces;

namespace UNIOOP.App.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly DataContextEF _entityFramework;
        public EnrollmentService(DataContextEF context)
        {
            _entityFramework = context;
        }

        public async Task<EnrollmentResponseDto?> GetSingleAsync(int studentId, int courseId)
        {
            return await GetEnrollmentResponseQuery()
                .Where(enrollment => enrollment.StudentID == studentId && enrollment.CourseID == courseId)
                .SingleOrDefaultAsync();
        }

        public async Task<List<EnrollmentResponseDto>> GetStudentCoursesAsync(int studentId)
        {
            return await GetEnrollmentResponseQuery()
               .Where(enrollment => enrollment.StudentID == studentId)
               .OrderBy(enrollment => enrollment.CourseName)
               .ToListAsync();
        }

        public async Task<List<EnrollmentResponseDto>> GetCourseStudentsAsync(int courseId)
        {
            return await GetEnrollmentResponseQuery()
               .Where(enrollment => enrollment.CourseID == courseId)
               .OrderBy(enrollment => enrollment.StudentID)
               .ToListAsync();
        }

        private IQueryable<EnrollmentResponseDto> GetEnrollmentResponseQuery()
        {
            IQueryable<EnrollmentResponseDto> query =
                from enrollment in _entityFramework.StudentCourses.AsNoTracking()

                join student in _entityFramework.Students.AsNoTracking()
                    on enrollment.StudentPersonnelID
                    equals student.PersonnelID

                join course in _entityFramework.Courses.AsNoTracking()
                    on enrollment.CourseID
                    equals course.CourseID

                join university in _entityFramework.Universities.AsNoTracking()
                    on course.UniversityID
                    equals university.UniversityID

                select new EnrollmentResponseDto
                {
                    StudentID = student.StudentID,
                    StudentName = student.FName + " " + student.LName,

                    CourseID = course.CourseID,
                    CourseName = course.CourseName,

                    UniversityID = university.UniversityID,
                    UniversityName = university.UniversityName
                };
            return query;
        }

        public async Task<EnrollmentResponseDto> EnrollAsync(CreateEnrollmentDto dto)
        {
            Student student = await _entityFramework.Students
                .AsNoTracking().SingleAsync(s => s.StudentID == dto.StudentID);

            var enrollment = new StudentCourse
            {
                StudentPersonnelID = student.PersonnelID,
                CourseID = dto.CourseID
            };

            _entityFramework.StudentCourses.Add(enrollment);

            await _entityFramework.SaveChangesAsync();

            return (await GetSingleAsync(dto.StudentID, dto.CourseID))!;
        }

        public async Task<bool> UnenrollAsync(int studentId, int courseId)
        {
            StudentCourse? enrollment = await (
                    from studentCourse in _entityFramework.StudentCourses
                    join student in _entityFramework.Students
                        on studentCourse.StudentPersonnelID
                        equals student.PersonnelID
                    where student.StudentID == studentId && studentCourse.CourseID == courseId
                    select studentCourse).SingleOrDefaultAsync();

            if (enrollment == null)
            {
                return false;
            }

            _entityFramework.StudentCourses.Remove(enrollment);

            await _entityFramework.SaveChangesAsync();

            return true;
        }
    }
}