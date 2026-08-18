using UNIOOP.App.Dtos.Students;
using UNIOOP.App.Models;
using UNIOOP.App.Services.Interfaces;
using UNIOOP.App.Helpers;
using UNIOOP.App.Repositories.Interfaces;
using AutoMapper;
using UNIOOP.App.Exceptions;

namespace UNIOOP.App.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IUniversityRepository _universityRepository;
        private readonly IPersonnelRepository _personnelRepository;
        private readonly IMapper _mapper;

        public StudentService(
            IStudentRepository studentRepository,
            IUniversityRepository universityRepository,
            IPersonnelRepository personnelRepository,
            IMapper mapper)
        {
            _studentRepository = studentRepository;
            _universityRepository = universityRepository;
            _personnelRepository = personnelRepository;
            _mapper = mapper;
        }

        public async Task<List<StudentResponseDto>> GetAllAsync()
        {
            var students = await _studentRepository.GetAllAsync();
            return _mapper.Map<List<StudentResponseDto>>(students);
        }

        public async Task<StudentResponseDto> GetSingleAsync(int studentId)
        {
            var student = await _studentRepository.GetByIdAsync(studentId);

            if (student is null)
            {
                throw new NotFoundException($"Student with ID {studentId} was not found.");
            }

            return _mapper.Map<StudentResponseDto>(student);
        }

        public async Task<StudentResponseDto> CreateAsync(CreateStudentDto dto)
        {
            string normalizedSsn = InputNormalizationHelper.NormalizeText(dto.SSN);
            string normalizedEmail = InputNormalizationHelper.NormalizeEmail(dto.Email);

            if (!await _universityRepository.ExistsAsync(dto.UniversityID))
            {
                throw new NotFoundException($"The selected university with Id {dto.UniversityID} does not exist");
            }

            if (await _personnelRepository.SSNExistsAsync(normalizedSsn))
            {
                throw new ConflictException($"The SSN {normalizedSsn} is already in use");
            }

            if (await _personnelRepository.EmailExistsAsync(normalizedEmail))
            {
                throw new ConflictException($"The email {normalizedEmail} is already in use");
            }

            var student = new Student
            {
                SSN = normalizedSsn,
                FName = InputNormalizationHelper.NormalizeText(dto.FName),
                LName = InputNormalizationHelper.NormalizeText(dto.LName),
                DateOfBirth = dto.DateOfBirth,
                Email = normalizedEmail,
                Major = InputNormalizationHelper.NormalizeText(dto.Major),
                GPA = dto.GPA,
                UniversityID = dto.UniversityID
            };

            await _studentRepository.AddAsync(student);
            await _studentRepository.SaveChangesAsync();

            return await GetSingleAsync(student.StudentID);
        }
        public async Task UpdateAsync(int studentId, UpdateStudentDto dto)
        {
            Student? existingStudent = await _studentRepository.GetByIdForUpdateAsync(studentId);

            if (existingStudent is null)
            {
                throw new NotFoundException($"Student with ID {studentId} was not found");
            }

            string normalizedEmail = InputNormalizationHelper.NormalizeEmail(dto.Email);

            if (await _personnelRepository.EmailExistsAsync(normalizedEmail, existingStudent.PersonnelID))
            {
                throw new ConflictException($"Another person already uses this email {normalizedEmail}");
            }

            if (!await _universityRepository.ExistsAsync(dto.UniversityID))
            {
                throw new NotFoundException($"The selected university with Id {dto.UniversityID} does not exist");
            }

            existingStudent.FName = InputNormalizationHelper.NormalizeText(dto.FName);
            existingStudent.LName = InputNormalizationHelper.NormalizeText(dto.LName);
            existingStudent.Email = normalizedEmail;
            existingStudent.Major = InputNormalizationHelper.NormalizeText(dto.Major);
            existingStudent.DateOfBirth = dto.DateOfBirth;
            existingStudent.GPA = dto.GPA;
            existingStudent.UniversityID = dto.UniversityID;

            await _studentRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int studentId)
        {
            Student? existingStudent = await _studentRepository.GetByIdForUpdateAsync(studentId);

            if (existingStudent is null)
            {
                throw new NotFoundException($"Student with ID {studentId} was not found");
            }

            if (await _studentRepository.HasEnrollmentsAsync(studentId))
            {
                throw new ConflictException("The student cannot be deleted while enrolled in courses");
            }

            _studentRepository.Remove(existingStudent);

            await _studentRepository.SaveChangesAsync();
        }
    }
}