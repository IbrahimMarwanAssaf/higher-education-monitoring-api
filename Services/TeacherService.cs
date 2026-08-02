using Microsoft.EntityFrameworkCore;
using UNIOOP.App.Data;
using UNIOOP.App.Dtos.Teachers;
using UNIOOP.App.Models;
using UNIOOP.App.Services.Interfaces;
using UNIOOP.App.Helpers;

namespace UNIOOP.App.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly DataContextEF _entityFramework;

        public TeacherService(DataContextEF context)
        {
            _entityFramework = context;
        }

        public async Task<List<TeacherResponseDto>> GetAllAsync()
        {
            return await GetTeacherResponseQuery()
                .OrderBy(teacher => teacher.TeacherID)
                .ToListAsync();
        }
        public async Task<TeacherResponseDto?> GetSingleAsync(int teacherId)
        {
            return await GetTeacherResponseQuery()
                .Where(teacher => teacher.TeacherID == teacherId)
                .SingleOrDefaultAsync();
        }

        private IQueryable<TeacherResponseDto> GetTeacherResponseQuery()
        {
            IQueryable<TeacherResponseDto> query =
                from teacher in _entityFramework.Teachers.AsNoTracking()
                join university in _entityFramework.Universities.AsNoTracking()
                    on teacher.UniversityID equals university.UniversityID
                select new TeacherResponseDto
                {
                    TeacherID = teacher.TeacherID,
                    FName = teacher.FName,
                    LName = teacher.LName,
                    DateOfBirth = teacher.DateOfBirth,
                    Email = teacher.Email,
                    Department = teacher.Department,
                    Salary = teacher.Salary,

                    MinistryDegreeID = teacher.MinistryDegreeID,
                    UniversityID = teacher.UniversityID,
                    UniversityName = university.UniversityName
                };
            return query;
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
            _entityFramework.Teachers.Add(teacher);

            await _entityFramework.SaveChangesAsync();

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
            Teacher? existingTeacher = await _entityFramework.Teachers
                .SingleOrDefaultAsync(s => s.TeacherID == teacherId);

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

            await _entityFramework.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int teacherId)
        {
            Teacher? existingTeacher = await _entityFramework.Teachers
                .SingleOrDefaultAsync(s => s.TeacherID == teacherId);

            if (existingTeacher is null)
            {
                return false;
            }

            _entityFramework.Teachers.Remove(existingTeacher);

            await _entityFramework.SaveChangesAsync();

            return true;
        }
    }
}