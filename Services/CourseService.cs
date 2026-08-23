using AutoMapper;
using UNIOOP.App.Caching;
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
        private readonly IInMemoryCacheService _cacheService;

        public CourseService(ICourseRepository courseRepository,
            ITeacherRepository teacherRepository,
            ExceptionHelper exceptionHelper,
            IMapper mapper,
            IInMemoryCacheService cacheService)
        {
            _courseRepository = courseRepository;
            _teacherRepository = teacherRepository;
            _exceptionHelper = exceptionHelper;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<List<CourseResponseDto>> GetAllAsync()
        {
            const string cacheKey = "Courses:All";

            List<CourseResponseDto>? courses = await _cacheService
                .GetOrCreateAsync(cacheKey, async () =>
                    {
                        var courseEntities = await _courseRepository.GetAllAsync();
                        return _mapper.Map<List<CourseResponseDto>>(courseEntities);
                    });

            if (courses != null)
            {
                return courses;
            }
            else
            {
                return new List<CourseResponseDto>();
            }
        }

        public async Task<CourseResponseDto> GetSingleAsync(int courseId)
        {
            string cacheKey = $"Course:{courseId}";

            CourseResponseDto? course = await _cacheService
                .GetOrCreateAsync(cacheKey, async () =>
                    {
                        Course? courseEntity = await _courseRepository
                            .GetByIdAsync(courseId);

                        if (courseEntity is null)
                        {
                            return null;
                        }

                        return _mapper.Map<CourseResponseDto>(courseEntity);
                    });

            if (course is null)
            {
                throw _exceptionHelper.NotFound("Course", courseId);
            }

            return course;
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
                normalizedCourseName, dto.UniversityID);

            var course = new Course
            {
                CourseName = normalizedCourseName,
                Credits = dto.Credits,
                UniversityID = dto.UniversityID,
                TeacherPersonnelID = teacherPersonnelId
            };

            await _courseRepository.AddAsync(course);
            await _courseRepository.SaveChangesAsync();

            await _cacheService.RemoveAsync("Courses:All");

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

            await _exceptionHelper.EnsureUniversityExistsAsync(dto.UniversityID);

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

            string normalizedCourseName = InputNormalizationHelper.NormalizeText(dto.CourseName);

            await _exceptionHelper.EnsureCourseNameAvailableAsync(normalizedCourseName,
            dto.UniversityID, courseId);

            existingCourse.CourseName = normalizedCourseName;
            existingCourse.Credits = dto.Credits;
            existingCourse.UniversityID = dto.UniversityID;
            existingCourse.TeacherPersonnelID = teacherPersonnelId;

            await _courseRepository.SaveChangesAsync();

            await _cacheService.RemoveAsync($"Course:{courseId}");
            await _cacheService.RemoveAsync("Courses:All");
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

            await _cacheService.RemoveAsync($"Course:{courseId}");
            await _cacheService.RemoveAsync("Courses:All");
        }
    }
}