using Microsoft.EntityFrameworkCore;
using UNIOOP.App.Data;
using UNIOOP.App.Dtos.Teachers;
using UNIOOP.App.Models;
using UNIOOP.App.Services.Interfaces;

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
            return await (
                from teacher in _entityFramework.Teachers.AsNoTracking()
                join university in _entityFramework.Universities.AsNoTracking()
                    on teacher.UniversityID equals university.UniversityID
                orderby teacher.TeacherID
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
                }).ToListAsync();
        }
        public async Task<TeacherResponseDto?> GetSingleAsync(int teacherId)
        {
            return await (
             from teacher in _entityFramework.Teachers.AsNoTracking()

             join university in
                 _entityFramework.Universities.AsNoTracking()
                 on teacher.UniversityID equals university.UniversityID

             where teacher.TeacherID == teacherId

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
             }).SingleOrDefaultAsync();
        }
        public async Task<TeacherResponseDto> CreateAsync(CreateTeacherDto dto)
        {
            var teacher = new Teacher
            {
                SSN = dto.SSN,
                FName = dto.FName,
                LName = dto.LName,
                DateOfBirth = dto.DateOfBirth,
                Email = dto.Email,
                Department = dto.Department,
                Salary = dto.Salary,
                MinistryDegreeID = dto.MinistryDegreeID,
                UniversityID = dto.UniversityID
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

            existingTeacher.FName = dto.FName;
            existingTeacher.LName = dto.LName;
            existingTeacher.DateOfBirth = dto.DateOfBirth;
            existingTeacher.Email = dto.Email;
            existingTeacher.Department = dto.Department;
            existingTeacher.Salary = dto.Salary;
            existingTeacher.MinistryDegreeID = dto.MinistryDegreeID;
            existingTeacher.UniversityID = dto.UniversityID;

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