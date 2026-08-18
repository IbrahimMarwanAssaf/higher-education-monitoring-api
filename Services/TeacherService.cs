using UNIOOP.App.Dtos.Teachers;
using UNIOOP.App.Models;
using UNIOOP.App.Services.Interfaces;
using UNIOOP.App.Helpers;
using UNIOOP.App.Repositories.Interfaces;
using AutoMapper;
using UNIOOP.App.Exceptions;

namespace UNIOOP.App.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly IPersonnelRepository _personnelRepository;
        private readonly IUniversityRepository _universityRepository;
        private readonly IMapper _mapper;

        public TeacherService(ITeacherRepository teacherRepository,
        IPersonnelRepository personnelRepository,
        IUniversityRepository universityRepository,
        IMapper mapper)
        {
            _teacherRepository = teacherRepository;
            _personnelRepository = personnelRepository;
            _universityRepository = universityRepository;
            _mapper = mapper;
        }

        public async Task<List<TeacherResponseDto>> GetAllAsync()
        {
            var teachers = await _teacherRepository.GetAllAsync();
            return _mapper.Map<List<TeacherResponseDto>>(teachers);
        }
        public async Task<TeacherResponseDto> GetSingleAsync(int teacherId)
        {
            var teacher = await _teacherRepository.GetByIdAsync(teacherId);

            if (teacher is null)
            {
                throw new NotFoundException($"Teacher with ID {teacherId} was not found.");
            }

            return _mapper.Map<TeacherResponseDto>(teacher);
        }

        public async Task<TeacherResponseDto> CreateAsync(CreateTeacherDto dto)
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

            return await GetSingleAsync(teacher.TeacherID);
        }
        public async Task UpdateAsync(int teacherId, UpdateTeacherDto dto)
        {
            Teacher? existingTeacher = await _teacherRepository.GetByIdForUpdateAsync(teacherId);

            if (existingTeacher is null)
            {
                throw new NotFoundException($"Teacher with ID {teacherId} was not found");
            }

            string normalizedEmail = InputNormalizationHelper.NormalizeEmail(dto.Email);

            if (await _personnelRepository.EmailExistsAsync(normalizedEmail, existingTeacher.PersonnelID))
            {
                throw new ConflictException($"Another person already uses this email {normalizedEmail}");
            }

            if (!await _universityRepository.ExistsAsync(dto.UniversityID))
            {
                throw new NotFoundException($"The selected university with Id {dto.UniversityID} does not exist");
            }

            existingTeacher.FName = InputNormalizationHelper.NormalizeText(dto.FName);
            existingTeacher.LName = InputNormalizationHelper.NormalizeText(dto.LName);
            existingTeacher.Email = normalizedEmail;
            existingTeacher.Department = InputNormalizationHelper.NormalizeText(dto.Department);
            existingTeacher.DateOfBirth = dto.DateOfBirth;
            existingTeacher.Salary = dto.Salary;
            existingTeacher.UniversityID = dto.UniversityID;
            existingTeacher.MinistryDegreeID = dto.MinistryDegreeID;

            await _teacherRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int teacherId)
        {
            Teacher? existingTeacher = await _teacherRepository.GetByIdForUpdateAsync(teacherId);

            if (existingTeacher is null)
            {
                throw new NotFoundException($"Teacher with ID {teacherId} was not found");
            }

            _teacherRepository.Remove(existingTeacher);

            await _teacherRepository.SaveChangesAsync();
        }
    }
}