using AutoMapper;
using UNIOOP.App.Caching;
using UNIOOP.App.Dtos.Teachers;
using UNIOOP.App.Helpers;
using UNIOOP.App.Models;
using UNIOOP.App.Repositories.Interfaces;
using UNIOOP.App.Services.Interfaces;

namespace UNIOOP.App.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly ExceptionHelper _exceptionHelper;
        private readonly IMapper _mapper;
        private readonly IInMemoryCacheService _cacheService;

        public TeacherService(ITeacherRepository teacherRepository,
            ExceptionHelper exceptionHelper,
            IInMemoryCacheService cacheService,
            IMapper mapper)
        {
            _teacherRepository = teacherRepository;
            _exceptionHelper = exceptionHelper;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<List<TeacherResponseDto>> GetAllAsync()
        {
            const string cacheKey = "Teachers:All";

            List<TeacherResponseDto>? teachers = await _cacheService
                .GetOrCreateAsync(cacheKey, async () =>
                    {
                        var teacherEntities = await _teacherRepository.GetAllAsync();
                        return _mapper.Map<List<TeacherResponseDto>>(teacherEntities);
                    });

            if (teachers != null)
            {
                return teachers;
            }
            else
            {
                return new List<TeacherResponseDto>();
            }
        }

        public async Task<TeacherResponseDto> GetSingleAsync(int teacherId)
        {
            string cacheKey = $"Teacher:{teacherId}";

            TeacherResponseDto? teacher = await _cacheService
                .GetOrCreateAsync(cacheKey, async () =>
                    {
                        Teacher? teacherEntity = await _teacherRepository.GetByIdAsync(teacherId);

                        if (teacherEntity is null)
                        {
                            return null;
                        }

                        return _mapper.Map<TeacherResponseDto>(teacherEntity);
                    });

            if (teacher is null)
            {
                throw _exceptionHelper.NotFound("Teacher", teacherId);
            }

            return teacher;
        }
        public async Task<TeacherResponseDto> CreateAsync(CreateTeacherDto dto)
        {
            string normalizedSsn = InputNormalizationHelper.NormalizeText(dto.SSN);

            string normalizedEmail = InputNormalizationHelper.NormalizeEmail(dto.Email);

            await _exceptionHelper.EnsureUniversityExistsAsync(dto.UniversityID);
            await _exceptionHelper.EnsureSsnAvailableAsync(normalizedSsn);
            await _exceptionHelper.EnsureEmailAvailableAsync(normalizedEmail);

            var teacher = new Teacher
            {
                SSN = normalizedSsn,
                FName = InputNormalizationHelper.NormalizeText(dto.FName),
                LName = InputNormalizationHelper.NormalizeText(dto.LName),
                DateOfBirth = dto.DateOfBirth,
                Email = normalizedEmail,
                Department = InputNormalizationHelper.NormalizeText(dto.Department),
                Salary = dto.Salary,
                UniversityID = dto.UniversityID,
                MinistryDegreeID = dto.MinistryDegreeID
            };

            await _teacherRepository.AddAsync(teacher);
            await _teacherRepository.SaveChangesAsync();

            _cacheService.Remove("Teachers:All");

            return await GetSingleAsync(teacher.TeacherID);
        }

        public async Task UpdateAsync(int teacherId, UpdateTeacherDto dto)
        {
            Teacher? existingTeacher = await _teacherRepository.GetByIdForUpdateAsync(teacherId);

            if (existingTeacher is null)
            {
                throw _exceptionHelper.NotFound("Teacher", teacherId);
            }

            string normalizedEmail = InputNormalizationHelper.NormalizeEmail(dto.Email);

            await _exceptionHelper.EnsureEmailAvailableAsync(normalizedEmail, existingTeacher.PersonnelID);
            await _exceptionHelper.EnsureUniversityExistsAsync(dto.UniversityID);

            existingTeacher.FName = InputNormalizationHelper.NormalizeText(dto.FName);
            existingTeacher.LName = InputNormalizationHelper.NormalizeText(dto.LName);
            existingTeacher.Email = normalizedEmail;
            existingTeacher.Department = InputNormalizationHelper.NormalizeText(dto.Department);
            existingTeacher.DateOfBirth = dto.DateOfBirth;
            existingTeacher.Salary = dto.Salary;
            existingTeacher.UniversityID = dto.UniversityID;
            existingTeacher.MinistryDegreeID = dto.MinistryDegreeID;

            await _teacherRepository.SaveChangesAsync();

            _cacheService.Remove($"Teacher:{teacherId}");
            _cacheService.Remove("Teachers:All");
        }

        public async Task DeleteAsync(int teacherId)
        {
            Teacher? existingTeacher = await _teacherRepository.GetByIdForUpdateAsync(teacherId);

            if (existingTeacher is null)
            {
                throw _exceptionHelper.NotFound("Teacher", teacherId);
            }

            _teacherRepository.Remove(existingTeacher);
            await _teacherRepository.SaveChangesAsync();

            _cacheService.Remove($"Teacher:{teacherId}");
            _cacheService.Remove("Teachers:All");
        }
    }
}