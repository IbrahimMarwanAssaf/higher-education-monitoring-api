using AutoMapper;
using UNIOOP.App.Caching;
using UNIOOP.App.Dtos.Enrollments;
using UNIOOP.App.Exceptions;
using UNIOOP.App.Helpers;
using UNIOOP.App.Models;
using UNIOOP.App.Repositories.Interfaces;
using UNIOOP.App.Services.Interfaces;

namespace UNIOOP.App.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ExceptionHelper _exceptionHelper;
        private readonly IMapper _mapper;
        private readonly IInMemoryCacheService _cacheService;

        public EnrollmentService(IEnrollmentRepository enrollmentRepository,
            IStudentRepository studentRepository,
            ExceptionHelper exceptionHelper,
            IInMemoryCacheService cacheService,
            IMapper mapper)
        {
            _enrollmentRepository = enrollmentRepository;
            _studentRepository = studentRepository;
            _exceptionHelper = exceptionHelper;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<EnrollmentResponseDto> GetSingleAsync(int studentId, int courseId)
        {
            string cacheKey = $"Enrollment:{studentId}:{courseId}";

            EnrollmentResponseDto? enrollment = await _cacheService.GetOrCreateAsync(cacheKey, async () =>
                {
                    StudentCourse? enrollmentEntity = await _enrollmentRepository
                        .GetSingleAsync(studentId, courseId);

                    if (enrollmentEntity is null)
                    {
                        return null;
                    }

                    return _mapper.Map<EnrollmentResponseDto>(enrollmentEntity);
                });

            if (enrollment is null)
            {
                throw new NotFoundException($"Enrollment for student {studentId} " + $"in course {courseId} was not found.");
            }

            return enrollment;
        }

        public async Task<List<EnrollmentResponseDto>> GetStudentCoursesAsync(int studentId)
        {
            string cacheKey = $"StudentCourses:{studentId}";

            List<EnrollmentResponseDto>? enrollments = await _cacheService
                .GetOrCreateAsync(cacheKey, async () =>
                    {
                        await _exceptionHelper.EnsureStudentExistsAsync(studentId);

                        var enrollmentEntities = await _enrollmentRepository
                            .GetStudentCoursesAsync(studentId);

                        return _mapper.Map<List<EnrollmentResponseDto>>(enrollmentEntities);
                    });

            if (enrollments != null)
            {
                return enrollments;
            }
            else
            {
                return new List<EnrollmentResponseDto>();
            }
        }

        public async Task<List<EnrollmentResponseDto>> GetCourseStudentsAsync(int courseId)
        {
            string cacheKey = $"CourseStudents:{courseId}";

            List<EnrollmentResponseDto>? enrollments = await _cacheService
                .GetOrCreateAsync(cacheKey, async () =>
                    {
                        await _exceptionHelper.EnsureCourseExistsAsync(courseId);

                        var enrollmentEntities = await _enrollmentRepository
                            .GetCourseStudentsAsync(courseId);

                        return _mapper.Map<List<EnrollmentResponseDto>>(enrollmentEntities);
                    });

            if (enrollments != null)
            {
                return enrollments;
            }
            else
            {
                return new List<EnrollmentResponseDto>();
            }
        }

        public async Task<EnrollmentResponseDto> EnrollAsync(CreateEnrollmentDto dto)
        {
            await _exceptionHelper.EnsureStudentExistsAsync(dto.StudentID);
            await _exceptionHelper.EnsureCourseExistsAsync(dto.CourseID);
            await _exceptionHelper.EnsureStudentAndCourseSameUniversityAsync(
                dto.StudentID, dto.CourseID);
            await _exceptionHelper.EnsureEnrollmentDoesNotExistAsync(dto.StudentID, dto.CourseID);

            Student? student = await _studentRepository
                .GetByIdAsync(dto.StudentID);

            if (student is null)
            {
                throw _exceptionHelper.NotFound("Student", dto.StudentID);
            }

            var enrollment = new StudentCourse
            {
                StudentPersonnelID = student.PersonnelID,
                CourseID = dto.CourseID
            };

            await _enrollmentRepository.AddAsync(enrollment);
            await _enrollmentRepository.SaveChangesAsync();

            await _cacheService.RemoveAsync($"Enrollment:{dto.StudentID}:{dto.CourseID}");
            await _cacheService.RemoveAsync($"StudentCourses:{dto.StudentID}");
            await _cacheService.RemoveAsync($"CourseStudents:{dto.CourseID}");

            return await GetSingleAsync(dto.StudentID, dto.CourseID);
        }

        public async Task UnenrollAsync(int studentId, int courseId)
        {
            StudentCourse? enrollment = await _enrollmentRepository
                .GetSingleAsync(studentId, courseId);

            if (enrollment is null)
            {
                throw new NotFoundException($"Enrollment for student {studentId} " + $"in course {courseId} was not found.");
            }

            _enrollmentRepository.Remove(enrollment);
            await _enrollmentRepository.SaveChangesAsync();

            await _cacheService.RemoveAsync($"Enrollment:{studentId}:{courseId}");
            await _cacheService.RemoveAsync($"StudentCourses:{studentId}");
            await _cacheService.RemoveAsync($"CourseStudents:{courseId}");
        }
    }
}