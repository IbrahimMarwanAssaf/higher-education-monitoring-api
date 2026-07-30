using Microsoft.EntityFrameworkCore;
using UNIOOP.App.Data;
using UNIOOP.App.Dtos.Students;
using UNIOOP.App.Models;
using UNIOOP.App.Services.Interfaces;

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
            return await (
                from student in _entityFramework.Students.AsNoTracking()
                join university in _entityFramework.Universities.AsNoTracking()
                    on student.UniversityID equals university.UniversityID
                orderby student.StudentID
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
                }).ToListAsync();
        }
        public async Task<StudentResponseDto?> GetSingleAsync(int studentId)
        {
            return await (
             from student in _entityFramework.Students.AsNoTracking()

             join university in
                 _entityFramework.Universities.AsNoTracking()
                 on student.UniversityID equals university.UniversityID

             where student.StudentID == studentId

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
             }).SingleOrDefaultAsync();
        }
        public async Task<StudentResponseDto?> CreateAsync(CreateStudentDto dto)
        {
            var student = new Student
            {
                SSN = dto.SSN,
                FName = dto.FName,
                LName = dto.LName,
                DateOfBirth = dto.DateOfBirth,
                Email = dto.Email,
                Major = dto.Major,
                GPA = dto.GPA,
                UniversityID = dto.UniversityID
            };
            _entityFramework.Students.Add(student);

            await _entityFramework.SaveChangesAsync();

            return await GetSingleAsync(student.StudentID);
        }
        public async Task<bool> UpdateAsync(int studentId, UpdateStudentDto dto)
        {
            Student? existingStudent = await _entityFramework.Students
                .SingleOrDefaultAsync(s => s.StudentID == studentId);

            if (existingStudent is null)
            {
                return false;
            }

            existingStudent.FName = dto.FName;
            existingStudent.LName = dto.LName;
            existingStudent.DateOfBirth = dto.DateOfBirth;
            existingStudent.Email = dto.Email;
            existingStudent.Major = dto.Major;
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

        // public async Task<bool> HasDependenciesAsync(int studentId)
        // {

        // }

    }
}