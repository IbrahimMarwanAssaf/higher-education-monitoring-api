using Microsoft.EntityFrameworkCore;
using UNIOOP.App.Models;

namespace UNIOOP.App.Data.Seed;

public static class HigherEducationDbSeeder
{
    public static async Task SeedAsync(
        DataContextEF context,
        CancellationToken cancellationToken = default)
    {
        var strategy = context.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            // Prevent duplicate seeding.
            bool alreadySeeded = await context.Universities
            .AnyAsync(
                university =>
                    university.UniversityName ==
                    "Jordan University of Science and Technology",
                cancellationToken);

            if (alreadySeeded)
            {
                return;
            }

            await using var transaction =
                await context.Database.BeginTransactionAsync(
                    cancellationToken);

            // =================================================
            // 1. UNIVERSITIES
            // =================================================

            var just = new University
            {
                UniversityName =
                    "Jordan University of Science and Technology"
            };

            var jordanUniversity = new University
            {
                UniversityName = "University of Jordan"
            };

            var balqaUniversity = new University
            {
                UniversityName = "Al-Balqa Applied University"
            };

            context.Universities.AddRange(
                just,
                jordanUniversity,
                balqaUniversity);

            await context.SaveChangesAsync(cancellationToken);

            // UniversityID values are now generated.

            // =================================================
            // 2. TEACHERS
            // =================================================

            var teacher1 = new Teacher
            {
                SSN = "1111111111",
                FName = "Ahmad",
                LName = "Naser",
                DateOfBirth = new DateOnly(1979, 2, 11),
                Email = "ahmad.naser@just.edu.jo",
                Department = "Computer Science",
                Salary = 2500.00m,
                UniversityID = just.UniversityID,

                // External SQL Server ID for later integration.
                MinistryDegreeID = 5001
            };

            var teacher2 = new Teacher
            {
                SSN = "2222222222",
                FName = "Lina",
                LName = "Omar",
                DateOfBirth = new DateOnly(1983, 6, 22),
                Email = "lina.omar@just.edu.jo",
                Department = "Software Engineering",
                Salary = 2400.00m,
                UniversityID = just.UniversityID,
                MinistryDegreeID = 5002
            };

            var teacher3 = new Teacher
            {
                SSN = "3333333333",
                FName = "Khaled",
                LName = "Hassan",
                DateOfBirth = new DateOnly(1975, 3, 19),
                Email = "khaled.hassan@ju.edu.jo",
                Department = "Information Technology",
                Salary = 2300.00m,
                UniversityID = jordanUniversity.UniversityID,
                MinistryDegreeID = 5003
            };

            var teacher4 = new Teacher
            {
                SSN = "4444444444",
                FName = "Sara",
                LName = "Ali",
                DateOfBirth = new DateOnly(1988, 9, 7),
                Email = "sara.ali@ju.edu.jo",
                Department = "Data Science",
                Salary = 2700.00m,
                UniversityID = jordanUniversity.UniversityID,
                MinistryDegreeID = 5004
            };

            var teacher5 = new Teacher
            {
                SSN = "5555555555",
                FName = "Noor",
                LName = "Mahmoud",
                DateOfBirth = new DateOnly(1986, 12, 3),
                Email = "noor.mahmoud@bau.edu.jo",
                Department = "Pharmacy",
                Salary = 2100.00m,
                UniversityID = balqaUniversity.UniversityID,
                MinistryDegreeID = 5005
            };

            context.Teachers.AddRange(
                teacher1,
                teacher2,
                teacher3,
                teacher4,
                teacher5);

            await context.SaveChangesAsync(cancellationToken);

            // PersonnelID and TeacherID values are now generated.

            // =================================================
            // 3. STUDENTS
            // =================================================

            var student1 = new Student
            {
                SSN = "6111111111",
                FName = "Omar",
                LName = "Khalil",
                DateOfBirth = new DateOnly(2003, 4, 9),
                Email = "omar.khalil@student.just.edu.jo",
                Major = "Computer Science",
                GPA = 3.70m,
                UniversityID = just.UniversityID
            };

            var student2 = new Student
            {
                SSN = "6222222222",
                FName = "Huda",
                LName = "Yousef",
                DateOfBirth = new DateOnly(2002, 8, 15),
                Email = "huda.yousef@student.just.edu.jo",
                Major = "Software Engineering",
                GPA = 3.45m,
                UniversityID = just.UniversityID
            };

            var student3 = new Student
            {
                SSN = "6333333333",
                FName = "Yazan",
                LName = "Saleh",
                DateOfBirth = new DateOnly(2004, 1, 21),
                Email = "yazan.saleh@student.just.edu.jo",
                Major = "Computer Science",
                GPA = 3.10m,
                UniversityID = just.UniversityID
            };

            var student4 = new Student
            {
                SSN = "6444444444",
                FName = "Maya",
                LName = "Hamdan",
                DateOfBirth = new DateOnly(2003, 7, 17),
                Email = "maya.hamdan@student.ju.edu.jo",
                Major = "Data Science",
                GPA = 3.85m,
                UniversityID = jordanUniversity.UniversityID
            };

            var student5 = new Student
            {
                SSN = "6555555555",
                FName = "Ali",
                LName = "Qasem",
                DateOfBirth = new DateOnly(2002, 11, 8),
                Email = "ali.qasem@student.ju.edu.jo",
                Major = "Information Technology",
                GPA = 2.95m,
                UniversityID = jordanUniversity.UniversityID
            };

            var student6 = new Student
            {
                SSN = "6666666666",
                FName = "Rama",
                LName = "Nabil",
                DateOfBirth = new DateOnly(2004, 5, 12),
                Email = "rama.nabil@student.bau.edu.jo",
                Major = "Pharmacy",
                GPA = 3.60m,
                UniversityID = balqaUniversity.UniversityID
            };

            var student7 = new Student
            {
                SSN = "6777777777",
                FName = "Laith",
                LName = "Fares",
                DateOfBirth = new DateOnly(2003, 10, 27),
                Email = "laith.fares@student.bau.edu.jo",
                Major = "Pharmacy",
                GPA = 3.25m,
                UniversityID = balqaUniversity.UniversityID
            };

            context.Students.AddRange(
                student1,
                student2,
                student3,
                student4,
                student5,
                student6,
                student7);

            await context.SaveChangesAsync(cancellationToken);

            // PersonnelID and StudentID values are now generated.

            // =================================================
            // 4. GOVERNMENT OFFICERS
            // =================================================

            var officer1 = new GovernmentOfficer
            {
                SSN = "9000000001",
                FName = "Mariam",
                LName = "Saleh",
                DateOfBirth = new DateOnly(1980, 5, 14),
                Email = "mariam.saleh@mohe.gov.jo"
            };

            var officer2 = new GovernmentOfficer
            {
                SSN = "9000000002",
                FName = "Samer",
                LName = "Khatib",
                DateOfBirth = new DateOnly(1977, 9, 20),
                Email = "samer.khatib@mohe.gov.jo"
            };

            context.GovernmentOfficers.AddRange(
                officer1,
                officer2);

            await context.SaveChangesAsync(cancellationToken);

            // =================================================
            // 5. COURSES
            // =================================================

            var oopCourse = new Course
            {
                CourseName = "Object-Oriented Programming",
                Credits = 3,
                UniversityID = just.UniversityID,
                TeacherPersonnelID = teacher1.PersonnelID
            };

            var databaseCourse = new Course
            {
                CourseName = "Database Systems",
                Credits = 3,
                UniversityID = just.UniversityID,
                TeacherPersonnelID = teacher2.PersonnelID
            };

            var apiCourse = new Course
            {
                CourseName = "Web API Development",
                Credits = 3,
                UniversityID = just.UniversityID,
                TeacherPersonnelID = teacher2.PersonnelID
            };

            var analyticsCourse = new Course
            {
                CourseName = "Data Analytics",
                Credits = 3,
                UniversityID = jordanUniversity.UniversityID,
                TeacherPersonnelID = teacher4.PersonnelID
            };

            var cloudCourse = new Course
            {
                CourseName = "Cloud Computing",
                Credits = 3,
                UniversityID = jordanUniversity.UniversityID,
                TeacherPersonnelID = teacher3.PersonnelID
            };

            var pharmaceuticalCourse = new Course
            {
                CourseName = "Pharmaceutical Technology",
                Credits = 3,
                UniversityID = balqaUniversity.UniversityID,
                TeacherPersonnelID = teacher5.PersonnelID
            };

            var regulationsCourse = new Course
            {
                CourseName = "Pharmacy Regulations",
                Credits = 2,
                UniversityID = balqaUniversity.UniversityID,
                TeacherPersonnelID = teacher5.PersonnelID
            };

            context.Courses.AddRange(
                oopCourse,
                databaseCourse,
                apiCourse,
                analyticsCourse,
                cloudCourse,
                pharmaceuticalCourse,
                regulationsCourse);

            await context.SaveChangesAsync(cancellationToken);

            // CourseID values are now generated.

            // =================================================
            // 6. STUDENT-COURSE ENROLMENTS
            // =================================================

            context.StudentCourses.AddRange(
                // Omar
                new StudentCourse
                {
                    StudentPersonnelID = student1.PersonnelID,
                    CourseID = oopCourse.CourseID
                },
                new StudentCourse
                {
                    StudentPersonnelID = student1.PersonnelID,
                    CourseID = databaseCourse.CourseID
                },
                new StudentCourse
                {
                    StudentPersonnelID = student1.PersonnelID,
                    CourseID = apiCourse.CourseID
                },

                // Huda
                new StudentCourse
                {
                    StudentPersonnelID = student2.PersonnelID,
                    CourseID = oopCourse.CourseID
                },
                new StudentCourse
                {
                    StudentPersonnelID = student2.PersonnelID,
                    CourseID = databaseCourse.CourseID
                },
                new StudentCourse
                {
                    StudentPersonnelID = student2.PersonnelID,
                    CourseID = apiCourse.CourseID
                },

                // Yazan
                new StudentCourse
                {
                    StudentPersonnelID = student3.PersonnelID,
                    CourseID = oopCourse.CourseID
                },
                new StudentCourse
                {
                    StudentPersonnelID = student3.PersonnelID,
                    CourseID = databaseCourse.CourseID
                },

                // Maya
                new StudentCourse
                {
                    StudentPersonnelID = student4.PersonnelID,
                    CourseID = analyticsCourse.CourseID
                },
                new StudentCourse
                {
                    StudentPersonnelID = student4.PersonnelID,
                    CourseID = cloudCourse.CourseID
                },

                // Ali
                new StudentCourse
                {
                    StudentPersonnelID = student5.PersonnelID,
                    CourseID = analyticsCourse.CourseID
                },
                new StudentCourse
                {
                    StudentPersonnelID = student5.PersonnelID,
                    CourseID = cloudCourse.CourseID
                },

                // Rama
                new StudentCourse
                {
                    StudentPersonnelID = student6.PersonnelID,
                    CourseID = pharmaceuticalCourse.CourseID
                },
                new StudentCourse
                {
                    StudentPersonnelID = student6.PersonnelID,
                    CourseID = regulationsCourse.CourseID
                },

                // Laith
                new StudentCourse
                {
                    StudentPersonnelID = student7.PersonnelID,
                    CourseID = pharmaceuticalCourse.CourseID
                },
                new StudentCourse
                {
                    StudentPersonnelID = student7.PersonnelID,
                    CourseID = regulationsCourse.CourseID
                }
            );

            await context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        });
    }
}