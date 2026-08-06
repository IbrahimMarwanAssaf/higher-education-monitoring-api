using UNIOOP.App.Dtos.Courses;
using UNIOOP.App.Models;
using UNIOOP.App.Services.Interfaces;
using UNIOOP.App.Helpers;
using UNIOOP.App.Repositories.Interfaces;
using AutoMapper;

namespace UNIOOP.App.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IMapper _mapper;
        public CourseService(ICourseRepository courseRepository, IMapper mapper, ITeacherRepository teacherRepository)
        {
            _courseRepository = courseRepository;
            _teacherRepository = teacherRepository;
            _mapper = mapper;
        }

        public async Task<List<CourseResponseDto>> GetAllAsync()
        {
            var courses = await _courseRepository.GetAllAsync();
            return _mapper.Map<List<CourseResponseDto>>(courses);
        }
        public async Task<CourseResponseDto?> GetSingleAsync(int courseId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            return course == null ? null : _mapper.Map<CourseResponseDto>(course);
        }

        public async Task<CourseResponseDto> CreateAsync(CreateCourseDto dto)
        {
            long? teacherPersonnelId = null;

            if (dto.TeacherID.HasValue)
            {
                Teacher? teacher = await _teacherRepository.GetByIdAsync(dto.TeacherID.Value);

                if (teacher == null)
                {
                    throw new InvalidOperationException(
                        $"Teacher with ID {dto.TeacherID.Value} was not found.");
                }

                teacherPersonnelId = teacher.PersonnelID;
            }

            var course = new Course
            {
                CourseName = InputNormalizationHelper.NormalizeText(dto.CourseName),
                Credits = dto.Credits,
                UniversityID = dto.UniversityID,
                TeacherPersonnelID = teacherPersonnelId,
            };

            await _courseRepository.AddAsync(course);

            await _courseRepository.SaveChangesAsync();

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
            Course? existingCourse = await _courseRepository.GetByIdForUpdateAsync(courseId);

            if (existingCourse is null)
            {
                return false;
            }

            long? teacherPersonnelId = null;

            if (dto.TeacherID.HasValue)
            {
                Teacher? teacher = await _teacherRepository.GetByIdAsync(dto.TeacherID.Value);

                if (teacher == null)
                {
                    throw new InvalidOperationException(
                        $"Teacher with ID {dto.TeacherID.Value} was not found.");
                }

                teacherPersonnelId = teacher.PersonnelID;
            }

            existingCourse.CourseName = dto.CourseName;
            existingCourse.Credits = dto.Credits;
            existingCourse.UniversityID = dto.UniversityID;
            existingCourse.TeacherPersonnelID = teacherPersonnelId;

            await _courseRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int courseId)
        {
            Course? existingCourse = await _courseRepository.GetByIdForUpdateAsync(courseId);

            if (existingCourse is null)
            {
                return false;
            }

            _courseRepository.Remove(existingCourse);

            await _courseRepository.SaveChangesAsync();

            return true;
        }
    }
}