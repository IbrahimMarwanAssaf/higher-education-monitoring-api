using AutoMapper;
using UNIOOP.App.Caching;
using UNIOOP.App.Dtos.Students;
using UNIOOP.App.Helpers;
using UNIOOP.App.Models;
using UNIOOP.App.Repositories.Interfaces;
using UNIOOP.App.Services.Interfaces;

namespace UNIOOP.App.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ExceptionHelper _exceptionHelper;
        private readonly IMapper _mapper;
        private readonly IInMemoryCacheService _cacheService;

        public StudentService(IStudentRepository studentRepository,
            ExceptionHelper exceptionHelper,
            IMapper mapper,
            IInMemoryCacheService cacheService)
        {
            _studentRepository = studentRepository;
            _exceptionHelper = exceptionHelper;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<List<StudentResponseDto>> GetAllAsync()
        {
            const string cacheKey = "Students:All";

            List<StudentResponseDto>? students = await _cacheService
                .GetOrCreateAsync(cacheKey, async () =>
                    {
                        var studentEntities = await _studentRepository.GetAllAsync();
                        return _mapper.Map<List<StudentResponseDto>>(studentEntities);
                    });

            if (students != null)
            {
                return students;
            }
            else
            {
                return new List<StudentResponseDto>();
            }
        }

        public async Task<StudentResponseDto> GetSingleAsync(int studentId)
        {
            string cacheKey = $"Student:{studentId}";

            StudentResponseDto? student = await _cacheService
                .GetOrCreateAsync(cacheKey, async () =>
                    {
                        Student? studentEntity = await _studentRepository
                            .GetByIdAsync(studentId);

                        if (studentEntity is null)
                        {
                            return null;
                        }

                        return _mapper.Map<StudentResponseDto>(studentEntity);
                    });

            if (student is null)
            {
                throw _exceptionHelper.NotFound("Student", studentId);
            }

            return student;
        }

        public async Task<StudentResponseDto> CreateAsync(CreateStudentDto dto)
        {
            string normalizedSsn = InputNormalizationHelper.NormalizeText(dto.SSN);
            string normalizedEmail = InputNormalizationHelper.NormalizeEmail(dto.Email);

            await _exceptionHelper.EnsureUniversityExistsAsync(dto.UniversityID);
            await _exceptionHelper.EnsureSsnAvailableAsync(normalizedSsn);
            await _exceptionHelper.EnsureEmailAvailableAsync(normalizedEmail);

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

            await _cacheService.RemoveAsync("Students:All");

            return await GetSingleAsync(student.StudentID);
        }

        public async Task UpdateAsync(int studentId, UpdateStudentDto dto)
        {
            var existingStudent = await _studentRepository.GetByIdForUpdateAsync(studentId);

            if (existingStudent is null)
            {
                throw _exceptionHelper.NotFound("Student", studentId);
            }

            string normalizedEmail = InputNormalizationHelper.NormalizeEmail(dto.Email);

            await _exceptionHelper.EnsureEmailAvailableAsync(normalizedEmail, existingStudent.PersonnelID);
            await _exceptionHelper.EnsureUniversityExistsAsync(dto.UniversityID);

            existingStudent.FName = InputNormalizationHelper.NormalizeText(dto.FName);
            existingStudent.LName = InputNormalizationHelper.NormalizeText(dto.LName);
            existingStudent.Email = normalizedEmail;
            existingStudent.Major = InputNormalizationHelper.NormalizeText(dto.Major);
            existingStudent.DateOfBirth = dto.DateOfBirth;
            existingStudent.GPA = dto.GPA;
            existingStudent.UniversityID = dto.UniversityID;

            await _studentRepository.SaveChangesAsync();

            await _cacheService.RemoveAsync($"Student:{studentId}");
            await _cacheService.RemoveAsync("Students:All");
        }

        public async Task DeleteAsync(int studentId)
        {
            var existingStudent = await _studentRepository.GetByIdForUpdateAsync(studentId);

            if (existingStudent is null)
            {
                throw _exceptionHelper.NotFound("Student", studentId);
            }

            await _exceptionHelper.EnsureStudentCanBeDeletedAsync(studentId);

            _studentRepository.Remove(existingStudent);
            await _studentRepository.SaveChangesAsync();

            await _cacheService.RemoveAsync($"Student:{studentId}");
            await _cacheService.RemoveAsync("Students:All");
        }
    }
}