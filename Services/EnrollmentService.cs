using AutoMapper;
using UNIOOP.App.Dtos.Enrollments;
using UNIOOP.App.Models;
using UNIOOP.App.Repositories.Interfaces;
using UNIOOP.App.Services.Interfaces;

namespace UNIOOP.App.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IMapper _mapper;

        public EnrollmentService(IEnrollmentRepository enrollmentRepository, IStudentRepository studentRepository, IMapper mapper)
        {
            _enrollmentRepository = enrollmentRepository;
            _studentRepository = studentRepository;
            _mapper = mapper;
        }

        public async Task<EnrollmentResponseDto?> GetSingleAsync(int studentId, int courseId)
        {
            StudentCourse? enrollment = await _enrollmentRepository.GetSingleAsync(studentId, courseId);

            if (enrollment == null)
            {
                return null;
            }

            return _mapper.Map<EnrollmentResponseDto>(enrollment);
        }

        public async Task<List<EnrollmentResponseDto>> GetStudentCoursesAsync(int studentId)
        {
            var enrollments = await _enrollmentRepository.GetStudentCoursesAsync(studentId);
            return _mapper.Map<List<EnrollmentResponseDto>>(enrollments);
        }

        public async Task<List<EnrollmentResponseDto>> GetCourseStudentsAsync(int courseId)
        {
            var enrollments = await _enrollmentRepository.GetCourseStudentsAsync(courseId);
            return _mapper.Map<List<EnrollmentResponseDto>>(enrollments);
        }

        public async Task<EnrollmentResponseDto> EnrollAsync(CreateEnrollmentDto dto)
        {
            Student student = await _studentRepository.GetByIdAsync(dto.StudentID)
                ?? throw new InvalidOperationException(
                    $"Student with ID {dto.StudentID} does not exist.");

            var enrollment = new StudentCourse
            {
                StudentPersonnelID = student.PersonnelID,
                CourseID = dto.CourseID
            };

            await _enrollmentRepository.AddAsync(enrollment);

            await _enrollmentRepository.SaveChangesAsync();

            return (await GetSingleAsync(dto.StudentID, dto.CourseID))!;
        }

        public async Task<bool> UnenrollAsync(int studentId, int courseId)
        {
            StudentCourse? enrollment = await _enrollmentRepository.GetSingleAsync(studentId, courseId);

            if (enrollment == null)
            {
                return false;
            }

            _enrollmentRepository.Remove(enrollment);

            await _enrollmentRepository.SaveChangesAsync();

            return true;
        }
    }
}