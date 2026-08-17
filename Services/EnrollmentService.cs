using AutoMapper;
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
        private readonly IDatabaseValidationHelper _databaseValidationHelper;
        private readonly IMapper _mapper;

        public EnrollmentService(IEnrollmentRepository enrollmentRepository,
            IStudentRepository studentRepository,
            IDatabaseValidationHelper databaseValidationHelper,
            IMapper mapper)
        {
            _enrollmentRepository = enrollmentRepository;
            _studentRepository = studentRepository;
            _databaseValidationHelper = databaseValidationHelper;
            _mapper = mapper;
        }

        public async Task<EnrollmentResponseDto> GetSingleAsync(int studentId, int courseId)
        {
            StudentCourse? enrollment = await _enrollmentRepository.GetSingleAsync(studentId, courseId);

            if (enrollment is null)
            {
                throw new NotFoundException($"Enrollment for student {studentId} in course {courseId} was not found.");
            }

            return _mapper.Map<EnrollmentResponseDto>(enrollment);
        }

        public async Task<List<EnrollmentResponseDto>> GetStudentCoursesAsync(int studentId)
        {
            if (!await _databaseValidationHelper.StudentExistsAsync(studentId))
            {
                throw new NotFoundException($"Student with ID {studentId} was not found.");
            }

            var enrollments = await _enrollmentRepository.GetStudentCoursesAsync(studentId);
            return _mapper.Map<List<EnrollmentResponseDto>>(enrollments);
        }

        public async Task<List<EnrollmentResponseDto>> GetCourseStudentsAsync(int courseId)
        {
            if (!await _databaseValidationHelper.CourseExistsAsync(courseId))
            {
                throw new NotFoundException($"Course with ID {courseId} was not found.");
            }

            var enrollments = await _enrollmentRepository.GetCourseStudentsAsync(courseId);
            return _mapper.Map<List<EnrollmentResponseDto>>(enrollments);
        }

        public async Task<EnrollmentResponseDto> EnrollAsync(CreateEnrollmentDto dto)
        {
            Student? student = await _studentRepository.GetByIdAsync(dto.StudentID);

            if (student is null)
            {
                throw new NotFoundException($"Student with ID {dto.StudentID} was not found.");
            }

            if (!await _databaseValidationHelper.CourseExistsAsync(dto.CourseID))
            {
                throw new NotFoundException($"Course with ID {dto.CourseID} was not found.");
            }

            if (await _databaseValidationHelper.EnrollmentExistsAsync(dto.StudentID, dto.CourseID))
            {
                throw new ConflictException($"The student with id: {dto.StudentID} is already enrolled in the course with id: {dto.CourseID}");
            }

            if (!await _databaseValidationHelper.StudentAndCourseSameUniversityAsync(dto.StudentID, dto.CourseID))
            {
                throw new BadRequestException("The student and course must belong to the same university.");
            }

            var enrollment = new StudentCourse
            {
                StudentPersonnelID = student.PersonnelID,
                CourseID = dto.CourseID
            };

            await _enrollmentRepository.AddAsync(enrollment);
            await _enrollmentRepository.SaveChangesAsync();
            return await GetSingleAsync(dto.StudentID, dto.CourseID);
        }

        public async Task UnenrollAsync(int studentId, int courseId)
        {
            StudentCourse? enrollment = await _enrollmentRepository.GetSingleAsync(studentId, courseId);

            if (enrollment is null)
            {
                throw new NotFoundException($"Enrollment for student {studentId} in course {courseId} was not found.");
            }

            _enrollmentRepository.Remove(enrollment);
            await _enrollmentRepository.SaveChangesAsync();
        }
    }
}