using AutoMapper;
using UNIOOP.App.Dtos.Courses;
using UNIOOP.App.Exceptions;
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
        private readonly IDatabaseValidationHelper _databaseValidationHelper;
        private readonly IMapper _mapper;

        public CourseService(ICourseRepository courseRepository,
            ITeacherRepository teacherRepository,
            IDatabaseValidationHelper databaseValidationHelper,
            IMapper mapper)
        {
            _courseRepository = courseRepository;
            _teacherRepository = teacherRepository;
            _databaseValidationHelper = databaseValidationHelper;
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
                throw new NotFoundException($"Course with ID {courseId} was not found.");
            }

            return _mapper.Map<CourseResponseDto>(course);
        }

        public async Task<CourseResponseDto> CreateAsync(CreateCourseDto dto)
        {
            if (!await _databaseValidationHelper.UniversityExistsAsync(dto.UniversityID))
            {
                throw new NotFoundException($"The selected university with ID {dto.UniversityID} does not exist.");
            }

            if (dto.TeacherID.HasValue)
            {
                if (!await _databaseValidationHelper.TeacherExistsAsync(dto.TeacherID.Value))
                {
                    throw new NotFoundException($"The selected teacher with ID {dto.TeacherID.Value} does not exist.");
                }

                if (!await _databaseValidationHelper.TeacherBelongsToUniversityAsync(dto.TeacherID.Value, dto.UniversityID))
                {
                    throw new BadRequestException($"The teacher with ID {dto.TeacherID.Value} does not belong to university {dto.UniversityID}.");
                }
            }

            string normalizedCourseName = InputNormalizationHelper.NormalizeText(dto.CourseName);

            if (await _databaseValidationHelper.CourseNameExistsAsync(normalizedCourseName, dto.UniversityID))
            {
                throw new ConflictException(
                    $"The course '{normalizedCourseName}' already exists in university {dto.UniversityID}.");
            }

            long? teacherPersonnelId = null;

            if (dto.TeacherID.HasValue)
            {
                Teacher teacher = await _teacherRepository.GetByIdAsync(dto.TeacherID.Value)
                    ?? throw new NotFoundException($"The selected teacher with ID {dto.TeacherID.Value} does not exist.");
                teacherPersonnelId = teacher.PersonnelID;
            }

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
            Course? existingCourse = await _courseRepository.GetByIdForUpdateAsync(courseId);

            if (existingCourse is null)
            {
                throw new NotFoundException($"Course with ID {courseId} was not found.");
            }

            if (!await _databaseValidationHelper.UniversityExistsAsync(dto.UniversityID))
            {
                throw new NotFoundException($"The selected university with ID {dto.UniversityID} does not exist.");
            }

            if (dto.TeacherID.HasValue)
            {
                if (!await _databaseValidationHelper.TeacherExistsAsync(dto.TeacherID.Value))
                {
                    throw new NotFoundException($"The selected teacher with ID {dto.TeacherID.Value} does not exist.");
                }

                if (!await _databaseValidationHelper.TeacherBelongsToUniversityAsync(dto.TeacherID.Value, dto.UniversityID))
                {
                    throw new BadRequestException($"The teacher with ID {dto.TeacherID.Value} does not belong to university {dto.UniversityID}.");
                }
            }

            string normalizedCourseName = InputNormalizationHelper.NormalizeText(dto.CourseName);

            if (await _databaseValidationHelper.CourseNameExistsAsync(normalizedCourseName, dto.UniversityID, courseId))
            {
                throw new ConflictException($"The course '{normalizedCourseName}' already exists in university {dto.UniversityID}.");
            }

            long? teacherPersonnelId = null;

            if (dto.TeacherID.HasValue)
            {
                Teacher teacher = await _teacherRepository.GetByIdAsync(dto.TeacherID.Value)
                    ?? throw new NotFoundException($"The selected teacher with ID {dto.TeacherID.Value} does not exist.");
                teacherPersonnelId = teacher.PersonnelID;
            }

            existingCourse.CourseName = normalizedCourseName;
            existingCourse.Credits = dto.Credits;
            existingCourse.UniversityID = dto.UniversityID;
            existingCourse.TeacherPersonnelID = teacherPersonnelId;

            await _courseRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int courseId)
        {
            Course? existingCourse = await _courseRepository.GetByIdForUpdateAsync(courseId);

            if (existingCourse is null)
            {
                throw new NotFoundException($"Course with ID {courseId} was not found.");
            }

            if (await _databaseValidationHelper.CourseHasEnrollmentsAsync(courseId))
            {
                throw new ConflictException($"Course with ID {courseId} cannot be deleted while students are enrolled in it.");
            }

            _courseRepository.Remove(existingCourse);
            await _courseRepository.SaveChangesAsync();
        }
    }
}