using Microsoft.EntityFrameworkCore;
using UNIOOP.App.Data;
using UNIOOP.App.Dtos.Courses;
using UNIOOP.App.Models;
using UNIOOP.App.Services.Interfaces;

namespace UNIOOP.App.Services
{
    public class CourseService : ICourseService
    {
        private readonly DataContextEF _entityFramework;

        public CourseService(DataContextEF context)
        {
            _entityFramework = context;
        }

        public async Task<List<CourseResponseDto>> GetAllAsync()
        {
            return await (
                from course in _entityFramework.Courses.AsNoTracking()
                join university in _entityFramework.Universities.AsNoTracking()
                    on course.UniversityID equals university.UniversityID
                join teacher in _entityFramework.Teachers.AsNoTracking()
                    on course.TeacherPersonnelID equals teacher.PersonnelID
                        into teacherGroup
                from teacher in teacherGroup.DefaultIfEmpty()
                orderby course.CourseID
                select new CourseResponseDto
                {
                    CourseID = course.CourseID,
                    CourseName = course.CourseName,
                    Credits = course.Credits,

                    UniversityID = course.UniversityID,
                    UniversityName = university.UniversityName,

                    TeacherID = teacher == null ? null : teacher.TeacherID,
                    TeacherName = teacher == null ? null : teacher.FName + " " + teacher.LName
                }).ToListAsync();
        }
        public async Task<CourseResponseDto?> GetSingleAsync(int courseId)
        {
            return await (
                from course in _entityFramework.Courses.AsNoTracking()
                join university in _entityFramework.Universities.AsNoTracking()
                    on course.UniversityID equals university.UniversityID
                join teacher in _entityFramework.Teachers.AsNoTracking()
                    on course.TeacherPersonnelID equals teacher.PersonnelID
                        into teacherGroup
                from teacher in teacherGroup.DefaultIfEmpty()
                where course.CourseID == courseId
                select new CourseResponseDto
                {
                    CourseID = course.CourseID,
                    CourseName = course.CourseName,
                    Credits = course.Credits,

                    UniversityID = course.UniversityID,
                    UniversityName = university.UniversityName,

                    TeacherID = teacher == null ? null : teacher.TeacherID,
                    TeacherName = teacher == null ? null : teacher.FName + " " + teacher.LName
                }).SingleOrDefaultAsync();
        }
        public async Task<CourseResponseDto> CreateAsync(CreateCourseDto dto)
        {
            long? teacherPersonnelId = null;

            if (dto.TeacherID.HasValue)
            {
                Teacher teacher = await _entityFramework.Teachers.AsNoTracking()
                    .SingleAsync(t => t.TeacherID == dto.TeacherID.Value);

                teacherPersonnelId = teacher.PersonnelID;
            }

            var course = new Course
            {
                CourseName = dto.CourseName,
                Credits = dto.Credits,
                UniversityID = dto.UniversityID,
                TeacherPersonnelID = teacherPersonnelId,
            };
            _entityFramework.Courses.Add(course);

            await _entityFramework.SaveChangesAsync();

            CourseResponseDto? createdCourse = await GetSingleAsync(course.CourseID);

            if (createdCourse is null)
            {
                throw new InvalidOperationException(
                    "The course was created but could not be retrieved.");
            }

            return createdCourse;
        }
        public async Task<bool> UpdateAsync(int courseId, UpdateCourseDto dto)
        {
            Course? existingCourse = await _entityFramework.Courses
                .SingleOrDefaultAsync(s => s.CourseID == courseId);

            if (existingCourse is null)
            {
                return false;
            }

            long? teacherPersonnelId = null;

            if (dto.TeacherID.HasValue)
            {
                Teacher teacher = await _entityFramework.Teachers.AsNoTracking()
                    .SingleAsync(t => t.TeacherID == dto.TeacherID.Value);

                teacherPersonnelId = teacher.PersonnelID;
            }

            existingCourse.CourseName = dto.CourseName;
            existingCourse.Credits = dto.Credits;
            existingCourse.UniversityID = dto.UniversityID;
            existingCourse.TeacherPersonnelID = teacherPersonnelId;

            await _entityFramework.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int courseId)
        {
            Course? existingCourse = await _entityFramework.Courses
                .SingleOrDefaultAsync(s => s.CourseID == courseId);

            if (existingCourse is null)
            {
                return false;
            }

            _entityFramework.Courses.Remove(existingCourse);

            await _entityFramework.SaveChangesAsync();

            return true;
        }

        // public async Task<bool> HasDependenciesAsync(int courseId)
        // {

        // }

    }
}