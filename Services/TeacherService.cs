using UNIOOP.App.Dtos.Teachers;
using UNIOOP.App.Models;
using UNIOOP.App.Services.Interfaces;
using UNIOOP.App.Helpers;
using UNIOOP.App.Repositories.Interfaces;
using AutoMapper;

namespace UNIOOP.App.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly IMapper _mapper;

        public TeacherService(ITeacherRepository teacherRepository, IMapper mapper)
        {
            _teacherRepository = teacherRepository;
            _mapper = mapper;
        }

        public async Task<List<TeacherResponseDto>> GetAllAsync()
        {
            var teachers = await _teacherRepository.GetAllAsync();
            return _mapper.Map<List<TeacherResponseDto>>(teachers);
        }
        public async Task<TeacherResponseDto?> GetSingleAsync(int teacherId)
        {
            var teacher = await _teacherRepository.GetByIdAsync(teacherId);
            return teacher == null ? null : _mapper.Map<TeacherResponseDto>(teacher);
        }

        public async Task<TeacherResponseDto> CreateAsync(CreateTeacherDto dto)
        {
            var teacher = new Teacher
            {
                SSN = InputNormalizationHelper.NormalizeSsn(dto.SSN),
                FName = InputNormalizationHelper.NormalizeText(dto.FName),
                LName = InputNormalizationHelper.NormalizeText(dto.LName),
                DateOfBirth = dto.DateOfBirth,
                Email = InputNormalizationHelper.NormalizeEmail(dto.Email),
                Department = InputNormalizationHelper.NormalizeText(dto.Department),
                Salary = dto.Salary,
                UniversityID = dto.UniversityID,
                MinistryDegreeID = dto.MinistryDegreeID
            };
            await _teacherRepository.AddAsync(teacher);

            await _teacherRepository.SaveChangesAsync();

            TeacherResponseDto? createdTeacher = await GetSingleAsync(teacher.TeacherID);

            if (createdTeacher is null)
            {
                throw new InvalidOperationException(
                    "The teacher was created but could not be retrieved.");
            }

            return createdTeacher;
        }
        public async Task<bool> UpdateAsync(int teacherId, UpdateTeacherDto dto)
        {
            Teacher? existingTeacher = await _teacherRepository.GetByIdForUpdateAsync(teacherId);

            if (existingTeacher is null)
            {
                return false;
            }

            existingTeacher.FName = InputNormalizationHelper.NormalizeText(dto.FName);
            existingTeacher.LName = InputNormalizationHelper.NormalizeText(dto.LName);
            existingTeacher.Email = InputNormalizationHelper.NormalizeEmail(dto.Email);
            existingTeacher.Department = InputNormalizationHelper.NormalizeText(dto.Department);
            existingTeacher.DateOfBirth = dto.DateOfBirth;
            existingTeacher.Salary = dto.Salary;
            existingTeacher.UniversityID = dto.UniversityID;
            existingTeacher.MinistryDegreeID = dto.MinistryDegreeID;

            await _teacherRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int teacherId)
        {
            Teacher? existingTeacher = await _teacherRepository.GetByIdForUpdateAsync(teacherId);

            if (existingTeacher is null)
            {
                return false;
            }

            _teacherRepository.Remove(existingTeacher);

            await _teacherRepository.SaveChangesAsync();

            return true;
        }
    }
}