using AutoMapper;
using UNIOOP.App.Dtos.Courses;
using UNIOOP.App.Helpers;
using UNIOOP.App.Models;
using UNIOOP.App.Repositories.Interfaces;
using UNIOOP.App.Services.Interfaces;

namespace UNIOOP.App.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly ExceptionHelper _exceptionHelper;
        private readonly IMapper _mapper;

        public CourseService(ICourseRepository courseRepository,
            ITeacherRepository teacherRepository,
            ExceptionHelper exceptionHelper,
            IMapper mapper)
        {
            _courseRepository = courseRepository;
            _teacherRepository = teacherRepository;
            _exceptionHelper = exceptionHelper;
            _mapper = mapper;
        }

        public async Task<List<CourseResponseDto>> GetAllAsync()
        {
            var courses = await _courseRepository.GetAllAsync();
            return _mapper.Map<List<CourseResponseDto>>(courses);
        }

        public async Task<CourseResponseDto> GetSingleAsync(int courseId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);

            if (course is null)
            {
                throw _exceptionHelper.NotFound("Course", courseId);
            }

            return _mapper.Map<CourseResponseDto>(course);
        }

        public async Task<CourseResponseDto> CreateAsync(CreateCourseDto dto)
        {
            await _exceptionHelper.EnsureUniversityExistsAsync(dto.UniversityID);

            long? teacherPersonnelId = null;

            if (dto.TeacherID.HasValue)
            {
                Teacher? teacher = await _teacherRepository.GetByIdAsync(dto.TeacherID.Value);

                if (teacher is null)
                {
                    throw _exceptionHelper.NotFound("Teacher", dto.TeacherID.Value);
                }

                await _exceptionHelper.EnsureTeacherBelongsToUniversityAsync(
                    dto.TeacherID.Value, dto.UniversityID);

                teacherPersonnelId = teacher.PersonnelID;
            }

            string normalizedCourseName = InputNormalizationHelper.NormalizeText(
                dto.CourseName);

            await _exceptionHelper.EnsureCourseNameAvailableAsync(
                normalizedCourseName,
                dto.UniversityID);

            var course = new Course
            {
                CourseName = normalizedCourseName,
                Credits = dto.Credits,
                UniversityID = dto.UniversityID,
                TeacherPersonnelID = teacherPersonnelId
            };

            await _courseRepository.AddAsync(course);
            await _courseRepository.SaveChangesAsync();

            return await GetSingleAsync(course.CourseID);
        }

        public async Task UpdateAsync(int courseId, UpdateCourseDto dto)
        {
            Course? existingCourse = await _courseRepository
                .GetByIdForUpdateAsync(courseId);

            if (existingCourse is null)
            {
                throw _exceptionHelper.NotFound("Course", courseId);
            }

            await _exceptionHelper.EnsureUniversityExistsAsync(
                dto.UniversityID);

            long? teacherPersonnelId = null;

            if (dto.TeacherID.HasValue)
            {
                Teacher? teacher = await _teacherRepository.GetByIdAsync(
                    dto.TeacherID.Value);

                if (teacher is null)
                {
                    throw _exceptionHelper.NotFound("Teacher", dto.TeacherID.Value);
                }

                await _exceptionHelper.EnsureTeacherBelongsToUniversityAsync(
                    dto.TeacherID.Value, dto.UniversityID);

                teacherPersonnelId = teacher.PersonnelID;
            }

            string normalizedCourseName = InputNormalizationHelper.NormalizeText(
                dto.CourseName);

            await _exceptionHelper.EnsureCourseNameAvailableAsync(normalizedCourseName, dto.UniversityID,
                courseId);

            existingCourse.CourseName = normalizedCourseName;
            existingCourse.Credits = dto.Credits;
            existingCourse.UniversityID = dto.UniversityID;
            existingCourse.TeacherPersonnelID = teacherPersonnelId;

            await _courseRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int courseId)
        {
            Course? existingCourse = await _courseRepository
                .GetByIdForUpdateAsync(courseId);

            if (existingCourse is null)
            {
                throw _exceptionHelper.NotFound(
                    "Course",
                    courseId);
            }

            await _exceptionHelper.EnsureCourseCanBeDeletedAsync(courseId);

            _courseRepository.Remove(existingCourse);
            await _courseRepository.SaveChangesAsync();
        }
    }
}