using Microsoft.EntityFrameworkCore;
using UNIOOP.App.Data;
using UNIOOP.App.Dtos.Students;
using UNIOOP.App.Models;
using UNIOOP.App.Services.Interfaces;
using UNIOOP.App.Helpers;

namespace UNIOOP.App.Services
{
    public class StudentService : IStudentService
    {
        private readonly DataContextEF _entityFramework;

        public StudentService(DataContextEF context)
        {
            _entityFramework = context;
        }

        public async Task<List<StudentResponseDto>> GetAllAsync()
        {
            return await GetStudentResponseQuery()
                .OrderBy(student => student.StudentID)
                .ToListAsync();
        }
        public async Task<StudentResponseDto?> GetSingleAsync(int studentId)
        {
            return await GetStudentResponseQuery()
                .Where(student => student.StudentID == studentId)
                .SingleOrDefaultAsync();
        }
        private IQueryable<StudentResponseDto> GetStudentResponseQuery()
        {
            IQueryable<StudentResponseDto> query =
                from student in _entityFramework.Students.AsNoTracking()
                join university in _entityFramework.Universities.AsNoTracking()
                    on student.UniversityID equals university.UniversityID
                select new StudentResponseDto
                {
                    StudentID = student.StudentID,
                    FName = student.FName,
                    LName = student.LName,
                    DateOfBirth = student.DateOfBirth,
                    Email = student.Email,
                    Major = student.Major,
                    GPA = student.GPA,

                    UniversityID = student.UniversityID,
                    UniversityName = university.UniversityName
                };
            return query;
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

            _entityFramework.Students.Add(student);

            await _entityFramework.SaveChangesAsync();

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
            Student? existingStudent = await _entityFramework.Students
                .SingleOrDefaultAsync(s => s.StudentID == studentId);

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

            await _entityFramework.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int studentId)
        {
            Student? existingStudent = await _entityFramework.Students
                .SingleOrDefaultAsync(s => s.StudentID == studentId);

            if (existingStudent is null)
            {
                return false;
            }

            _entityFramework.Students.Remove(existingStudent);

            await _entityFramework.SaveChangesAsync();

            return true;
        }

    }
}