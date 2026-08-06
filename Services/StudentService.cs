using UNIOOP.App.Dtos.Students;
using UNIOOP.App.Models;
using UNIOOP.App.Services.Interfaces;
using UNIOOP.App.Helpers;
using UNIOOP.App.Repositories.Interfaces;
using AutoMapper;

namespace UNIOOP.App.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IMapper _mapper;

        public StudentService(IStudentRepository studentRepository, IMapper mapper)
        {
            _studentRepository = studentRepository;
            _mapper = mapper;
        }

        public async Task<List<StudentResponseDto>> GetAllAsync()
        {
            var students = await _studentRepository.GetAllAsync();

            return _mapper.Map<List<StudentResponseDto>>(students);
        }

        public async Task<StudentResponseDto?> GetSingleAsync(int studentId)
        {
            var student = await _studentRepository.GetByIdAsync(studentId);

            return student == null ? null : _mapper.Map<StudentResponseDto>(student);
        }
        public async Task<StudentResponseDto> CreateAsync(CreateStudentDto dto)
        {
            var student = new Student
            {
                SSN = InputNormalizationHelper.NormalizeSsn(dto.SSN),
                FName = InputNormalizationHelper.NormalizeText(dto.FName),
                LName = InputNormalizationHelper.NormalizeText(dto.LName),
                DateOfBirth = dto.DateOfBirth,
                Email = InputNormalizationHelper.NormalizeEmail(dto.Email),
                Major = InputNormalizationHelper.NormalizeText(dto.Major),
                GPA = dto.GPA,
                UniversityID = dto.UniversityID
            };

            await _studentRepository.AddAsync(student);
            await _studentRepository.SaveChangesAsync();

            StudentResponseDto? createdStudent = await GetSingleAsync(student.StudentID);

            if (createdStudent is null)
            {
                throw new InvalidOperationException(
                    "The student was created but could not be retrieved.");
            }

            return createdStudent;
        }
        public async Task<bool> UpdateAsync(int studentId, UpdateStudentDto dto)
        {
            Student? existingStudent = await _studentRepository.GetByIdForUpdateAsync(studentId);

            if (existingStudent is null)
            {
                return false;
            }

            existingStudent.FName = InputNormalizationHelper.NormalizeText(dto.FName);
            existingStudent.LName = InputNormalizationHelper.NormalizeText(dto.LName);
            existingStudent.Email = InputNormalizationHelper.NormalizeEmail(dto.Email);
            existingStudent.Major = InputNormalizationHelper.NormalizeText(dto.Major);
            existingStudent.DateOfBirth = dto.DateOfBirth;
            existingStudent.GPA = dto.GPA;
            existingStudent.UniversityID = dto.UniversityID;

            await _studentRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int studentId)
        {
            Student? existingStudent = await _studentRepository.GetByIdForUpdateAsync(studentId);

            if (existingStudent is null)
            {
                return false;
            }

            _studentRepository.Remove(existingStudent);

            await _studentRepository.SaveChangesAsync();

            return true;
        }

    }
}