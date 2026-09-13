using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using UNIOOP.App.Caching;
using UNIOOP.App.Dtos.GovernmentOfficers;
using UNIOOP.App.Helpers;
using UNIOOP.App.Models;
using UNIOOP.App.Repositories.Interfaces;
using UNIOOP.App.Services;
using UNIOOP.App.Services.Interfaces;

namespace UNIOOP.App.Tests.Unit.Helpers
{
    public class GovernmentOfficerServiceTestHelper
    {
        public Mock<ICurrentUserService> CurrentUserServiceMock { get; }
        public Mock<IGovernmentOfficerRepository> GovernmentOfficerRepositoryMock { get; }
        public Mock<IUserAccountRepository> UserAccountRepositoryMock { get; }
        public Mock<IUniversityRepository> UniversityRepositoryMock { get; }
        public Mock<IStudentRepository> StudentRepositoryMock { get; }
        public Mock<ITeacherRepository> TeacherRepositoryMock { get; }
        public Mock<ICourseRepository> CourseRepositoryMock { get; }
        public Mock<IPersonnelRepository> PersonnelRepositoryMock { get; }
        public Mock<IEnrollmentRepository> EnrollmentRepositoryMock { get; }
        public Mock<IInMemoryCacheService> CacheServiceMock { get; }
        public Mock<IMapper> MapperMock { get; }
        public PasswordHasher<UserAccount> PasswordHasher { get; }
        public GovernmentOfficerService Service { get; }

        public GovernmentOfficerServiceTestHelper(string currentUserRole)
        {
            CurrentUserServiceMock = new Mock<ICurrentUserService>();
            CurrentUserServiceMock.Setup(x => x.Role).Returns(currentUserRole);

            GovernmentOfficerRepositoryMock = new Mock<IGovernmentOfficerRepository>();
            UserAccountRepositoryMock = new Mock<IUserAccountRepository>();
            UniversityRepositoryMock = new Mock<IUniversityRepository>();
            StudentRepositoryMock = new Mock<IStudentRepository>();
            TeacherRepositoryMock = new Mock<ITeacherRepository>();
            CourseRepositoryMock = new Mock<ICourseRepository>();
            PersonnelRepositoryMock = new Mock<IPersonnelRepository>();
            EnrollmentRepositoryMock = new Mock<IEnrollmentRepository>();
            CacheServiceMock = new Mock<IInMemoryCacheService>();
            MapperMock = new Mock<IMapper>();
            PasswordHasher = new PasswordHasher<UserAccount>();

            var exceptionHelper = new ExceptionHelper(UniversityRepositoryMock.Object,
                StudentRepositoryMock.Object,
                TeacherRepositoryMock.Object,
                CourseRepositoryMock.Object,
                PersonnelRepositoryMock.Object,
                EnrollmentRepositoryMock.Object);

            Service = new GovernmentOfficerService(GovernmentOfficerRepositoryMock.Object,
                UserAccountRepositoryMock.Object,
                exceptionHelper,
                CacheServiceMock.Object,
                MapperMock.Object,
                PasswordHasher,
                CurrentUserServiceMock.Object);
        }

        public void SetupSuccessfulCreate()
        {
            PersonnelRepositoryMock.Setup(r => r.SSNExistsAsync("123456789")).ReturnsAsync(false);

            PersonnelRepositoryMock.Setup(r =>
                r.EmailExistsAsync("test@example.com", null)).ReturnsAsync(false);

            CacheServiceMock.Setup(c => c.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<Task<GovernmentOfficerResponseDto?>>>()))
                .Returns<string, Func<Task<GovernmentOfficerResponseDto?>>>((key, factory) => factory());

            GovernmentOfficerRepositoryMock.Setup(r => r.AddAsync(It.IsAny<GovernmentOfficer>()))
                .Callback<GovernmentOfficer>(officer => officer.OfficerID = 1)
                .Returns(Task.CompletedTask);

            GovernmentOfficerRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new GovernmentOfficer
                {
                    OfficerID = 1,
                    SSN = "123456789",
                    FName = "Test",
                    LName = "Officer",
                    DateOfBirth = new DateOnly(1990, 1, 1),
                    Email = "test@example.com"
                });

            MapperMock.Setup(m => m.Map<GovernmentOfficerResponseDto>(It.IsAny<GovernmentOfficer>()))
                .Returns(new GovernmentOfficerResponseDto
                {
                    OfficerID = 1,
                    SSN = "123456789",
                    FName = "Test",
                    LName = "Officer",
                    DateOfBirth = new DateOnly(1990, 1, 1),
                    Email = "test@example.com"
                });
        }

        public CreateGovernmentOfficerDto CreateDto(string role)
        {
            return new CreateGovernmentOfficerDto
            {
                SSN = "123456789",
                FName = "Test",
                LName = "Officer",
                DateOfBirth = new DateOnly(1990, 1, 1),
                Email = "test@example.com",
                Password = "TestPassword123!",
                Role = role
            };
        }
    }
}

