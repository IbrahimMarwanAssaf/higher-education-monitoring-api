using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using UNIOOP.App.Caching;
using UNIOOP.App.Constants;
using UNIOOP.App.Dtos.GovernmentOfficers;
using UNIOOP.App.Exceptions;
using UNIOOP.App.Helpers;
using UNIOOP.App.Models;
using UNIOOP.App.Repositories.Interfaces;
using UNIOOP.App.Services;
using UNIOOP.App.Services.Interfaces;

namespace UNIOOP.App.Tests.Unit
{
    public class GovernmentOfficerServiceTests
    {
        [Fact]
        public async Task CreateAsync_AdminCreatingAdmin_ThrowsBadRequestException()
        {
            // Arrange
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            currentUserServiceMock.Setup(x => x.Role)
                .Returns(RoleConstants.Admin);

            var dto = new CreateGovernmentOfficerDto
            {
                SSN = "123456789",
                FName = "Test",
                LName = "Officer",
                DateOfBirth = new DateOnly(1990, 1, 1),
                Email = "test@example.com",
                Password = "TestPassword123!",
                Role = RoleConstants.Admin
            };

            var governmentOfficerRepositoryMock = new Mock<IGovernmentOfficerRepository>();
            var userAccountRepositoryMock = new Mock<IUserAccountRepository>();
            var universityRepositoryMock = new Mock<IUniversityRepository>();
            var studentRepositoryMock = new Mock<IStudentRepository>();
            var teacherRepositoryMock = new Mock<ITeacherRepository>();
            var courseRepositoryMock = new Mock<ICourseRepository>();
            var personnelRepositoryMock = new Mock<IPersonnelRepository>();
            var enrollmentRepositoryMock = new Mock<IEnrollmentRepository>();

            var exceptionHelper = new ExceptionHelper(universityRepositoryMock.Object,
                studentRepositoryMock.Object,
                teacherRepositoryMock.Object,
                courseRepositoryMock.Object,
                personnelRepositoryMock.Object,
                enrollmentRepositoryMock.Object);

            var cacheServiceMock = new Mock<IInMemoryCacheService>();
            var mapperMock = new Mock<IMapper>();
            var passwordHasher = new PasswordHasher<UserAccount>();

            var governmentOfficerService = new GovernmentOfficerService(governmentOfficerRepositoryMock.Object,
                userAccountRepositoryMock.Object,
                exceptionHelper,
                cacheServiceMock.Object,
                mapperMock.Object,
                passwordHasher,
                currentUserServiceMock.Object);

            //Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(() => governmentOfficerService.CreateAsync(dto));
            // Assert
            Assert.Equal("An Admin cannot create another Admin.", exception.Message);

            governmentOfficerRepositoryMock.Verify(r => r.AddAsync(It.IsAny<GovernmentOfficer>()), Times.Never);
            userAccountRepositoryMock.Verify(r => r.AddAsync(It.IsAny<UserAccount>()), Times.Never);
            governmentOfficerRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_SuperAdminCreatingAdmin_CreatesOfficer()
        {
            // Arrange
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            currentUserServiceMock.Setup(x => x.Role).Returns(RoleConstants.SuperAdmin);

            var dto = new CreateGovernmentOfficerDto
            {
                SSN = "123456789",
                FName = "Test",
                LName = "Officer",
                DateOfBirth = new DateOnly(1990, 1, 1),
                Email = "test@example.com",
                Password = "TestPassword123!",
                Role = RoleConstants.Admin
            };

            var governmentOfficerRepositoryMock = new Mock<IGovernmentOfficerRepository>();
            var userAccountRepositoryMock = new Mock<IUserAccountRepository>();
            var universityRepositoryMock = new Mock<IUniversityRepository>();
            var studentRepositoryMock = new Mock<IStudentRepository>();
            var teacherRepositoryMock = new Mock<ITeacherRepository>();
            var courseRepositoryMock = new Mock<ICourseRepository>();
            var personnelRepositoryMock = new Mock<IPersonnelRepository>();
            var enrollmentRepositoryMock = new Mock<IEnrollmentRepository>();

            var exceptionHelper = new ExceptionHelper(universityRepositoryMock.Object,
                studentRepositoryMock.Object,
                teacherRepositoryMock.Object,
                courseRepositoryMock.Object,
                personnelRepositoryMock.Object,
                enrollmentRepositoryMock.Object);

            var cacheServiceMock = new Mock<IInMemoryCacheService>();
            var mapperMock = new Mock<IMapper>();
            var passwordHasher = new PasswordHasher<UserAccount>();

            var governmentOfficerService = new GovernmentOfficerService(governmentOfficerRepositoryMock.Object,
                userAccountRepositoryMock.Object,
                exceptionHelper,
                cacheServiceMock.Object,
                mapperMock.Object,
                passwordHasher,
                currentUserServiceMock.Object);

            personnelRepositoryMock.Setup(r => r.SSNExistsAsync("123456789"))
                .ReturnsAsync(false);
            personnelRepositoryMock.Setup(r => r.EmailExistsAsync("test@example.com", null))
                .ReturnsAsync(false);

            cacheServiceMock.Setup(c => c.GetOrCreateAsync(It.IsAny<string>(),
                It.IsAny<Func<Task<GovernmentOfficerResponseDto?>>>()))
                    .Returns<string, Func<Task<GovernmentOfficerResponseDto?>>>((key, factory) => factory());

            governmentOfficerRepositoryMock.Setup(r => r.AddAsync(It.IsAny<GovernmentOfficer>()))
                .Callback<GovernmentOfficer>(officer =>
                    {
                        officer.OfficerID = 1;
                    }).Returns(Task.CompletedTask);

            governmentOfficerRepositoryMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new GovernmentOfficer
                {
                    OfficerID = 1,
                    SSN = "123456789",
                    FName = "Test",
                    LName = "Officer",
                    DateOfBirth = new DateOnly(1990, 1, 1),
                    Email = "test@example.com"
                });

            mapperMock.Setup(m => m.Map<GovernmentOfficerResponseDto>(It.IsAny<GovernmentOfficer>()))
                .Returns(new GovernmentOfficerResponseDto
                {
                    OfficerID = 1,
                    SSN = "123456789",
                    FName = "Test",
                    LName = "Officer",
                    DateOfBirth = new DateOnly(1990, 1, 1),
                    Email = "test@example.com"
                });

            // Act
            var result = await governmentOfficerService.CreateAsync(dto);

            // Assert
            Assert.Equal(1, result.OfficerID);
            Assert.Equal("test@example.com", result.Email);
            Assert.Equal("Test", result.FName);
            Assert.Equal("Officer", result.LName);

            governmentOfficerRepositoryMock.Verify(r => r.AddAsync(It.IsAny<GovernmentOfficer>()), Times.Once);
            userAccountRepositoryMock.Verify(r => r.AddAsync(It.Is<UserAccount>(account =>
                account.Role == RoleConstants.Admin)), Times.Once);
            governmentOfficerRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_AdminCreatingManager_CreatesOfficer()
        {
            // Arrange
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            currentUserServiceMock.Setup(x => x.Role).Returns(RoleConstants.Admin);

            var dto = new CreateGovernmentOfficerDto
            {
                SSN = "123456789",
                FName = "Test",
                LName = "Officer",
                DateOfBirth = new DateOnly(1990, 1, 1),
                Email = "test@example.com",
                Password = "TestPassword123!",
                Role = RoleConstants.Manager
            };

            var governmentOfficerRepositoryMock = new Mock<IGovernmentOfficerRepository>();
            var userAccountRepositoryMock = new Mock<IUserAccountRepository>();
            var universityRepositoryMock = new Mock<IUniversityRepository>();
            var studentRepositoryMock = new Mock<IStudentRepository>();
            var teacherRepositoryMock = new Mock<ITeacherRepository>();
            var courseRepositoryMock = new Mock<ICourseRepository>();
            var personnelRepositoryMock = new Mock<IPersonnelRepository>();
            var enrollmentRepositoryMock = new Mock<IEnrollmentRepository>();

            var exceptionHelper = new ExceptionHelper(universityRepositoryMock.Object,
                studentRepositoryMock.Object,
                teacherRepositoryMock.Object,
                courseRepositoryMock.Object,
                personnelRepositoryMock.Object,
                enrollmentRepositoryMock.Object);

            var cacheServiceMock = new Mock<IInMemoryCacheService>();
            var mapperMock = new Mock<IMapper>();
            var passwordHasher = new PasswordHasher<UserAccount>();

            var governmentOfficerService = new GovernmentOfficerService(governmentOfficerRepositoryMock.Object,
                userAccountRepositoryMock.Object,
                exceptionHelper,
                cacheServiceMock.Object,
                mapperMock.Object,
                passwordHasher,
                currentUserServiceMock.Object);

            personnelRepositoryMock.Setup(r => r.SSNExistsAsync("123456789"))
                .ReturnsAsync(false);
            personnelRepositoryMock.Setup(r => r.EmailExistsAsync("test@example.com", null))
                .ReturnsAsync(false);

            cacheServiceMock.Setup(c => c.GetOrCreateAsync(It.IsAny<string>(),
                It.IsAny<Func<Task<GovernmentOfficerResponseDto?>>>()))
                    .Returns<string, Func<Task<GovernmentOfficerResponseDto?>>>((key, factory) => factory());

            governmentOfficerRepositoryMock.Setup(r => r.AddAsync(It.IsAny<GovernmentOfficer>()))
                .Callback<GovernmentOfficer>(officer =>
                    {
                        officer.OfficerID = 1;
                    }).Returns(Task.CompletedTask);

            governmentOfficerRepositoryMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new GovernmentOfficer
                {
                    OfficerID = 1,
                    SSN = "123456789",
                    FName = "Test",
                    LName = "Officer",
                    DateOfBirth = new DateOnly(1990, 1, 1),
                    Email = "test@example.com"
                });

            mapperMock.Setup(m => m.Map<GovernmentOfficerResponseDto>(It.IsAny<GovernmentOfficer>()))
                .Returns(new GovernmentOfficerResponseDto
                {
                    OfficerID = 1,
                    SSN = "123456789",
                    FName = "Test",
                    LName = "Officer",
                    DateOfBirth = new DateOnly(1990, 1, 1),
                    Email = "test@example.com"
                });

            // Act
            var result = await governmentOfficerService.CreateAsync(dto);

            // Assert
            Assert.Equal(1, result.OfficerID);
            Assert.Equal("test@example.com", result.Email);
            Assert.Equal("Test", result.FName);
            Assert.Equal("Officer", result.LName);

            governmentOfficerRepositoryMock.Verify(r => r.AddAsync(It.IsAny<GovernmentOfficer>()), Times.Once);
            userAccountRepositoryMock.Verify(r => r.AddAsync(It.Is<UserAccount>(account =>
                account.Role == RoleConstants.Manager)), Times.Once);
            governmentOfficerRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_AdminCreatingUser_CreatesOfficer()
        {
            // Arrange
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            currentUserServiceMock.Setup(x => x.Role).Returns(RoleConstants.Admin);

            var dto = new CreateGovernmentOfficerDto
            {
                SSN = "123456789",
                FName = "Test",
                LName = "Officer",
                DateOfBirth = new DateOnly(1990, 1, 1),
                Email = "test@example.com",
                Password = "TestPassword123!",
                Role = RoleConstants.User
            };

            var governmentOfficerRepositoryMock = new Mock<IGovernmentOfficerRepository>();
            var userAccountRepositoryMock = new Mock<IUserAccountRepository>();
            var universityRepositoryMock = new Mock<IUniversityRepository>();
            var studentRepositoryMock = new Mock<IStudentRepository>();
            var teacherRepositoryMock = new Mock<ITeacherRepository>();
            var courseRepositoryMock = new Mock<ICourseRepository>();
            var personnelRepositoryMock = new Mock<IPersonnelRepository>();
            var enrollmentRepositoryMock = new Mock<IEnrollmentRepository>();

            var exceptionHelper = new ExceptionHelper(universityRepositoryMock.Object,
                studentRepositoryMock.Object,
                teacherRepositoryMock.Object,
                courseRepositoryMock.Object,
                personnelRepositoryMock.Object,
                enrollmentRepositoryMock.Object);

            var cacheServiceMock = new Mock<IInMemoryCacheService>();
            var mapperMock = new Mock<IMapper>();
            var passwordHasher = new PasswordHasher<UserAccount>();

            var governmentOfficerService = new GovernmentOfficerService(governmentOfficerRepositoryMock.Object,
                userAccountRepositoryMock.Object,
                exceptionHelper,
                cacheServiceMock.Object,
                mapperMock.Object,
                passwordHasher,
                currentUserServiceMock.Object);

            personnelRepositoryMock.Setup(r => r.SSNExistsAsync("123456789"))
                .ReturnsAsync(false);
            personnelRepositoryMock.Setup(r => r.EmailExistsAsync("test@example.com", null))
                .ReturnsAsync(false);

            cacheServiceMock.Setup(c => c.GetOrCreateAsync(It.IsAny<string>(),
                It.IsAny<Func<Task<GovernmentOfficerResponseDto?>>>()))
                    .Returns<string, Func<Task<GovernmentOfficerResponseDto?>>>((key, factory) => factory());

            governmentOfficerRepositoryMock.Setup(r => r.AddAsync(It.IsAny<GovernmentOfficer>()))
                .Callback<GovernmentOfficer>(officer =>
                    {
                        officer.OfficerID = 1;
                    }).Returns(Task.CompletedTask);

            governmentOfficerRepositoryMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new GovernmentOfficer
                {
                    OfficerID = 1,
                    SSN = "123456789",
                    FName = "Test",
                    LName = "Officer",
                    DateOfBirth = new DateOnly(1990, 1, 1),
                    Email = "test@example.com"
                });

            mapperMock.Setup(m => m.Map<GovernmentOfficerResponseDto>(It.IsAny<GovernmentOfficer>()))
                .Returns(new GovernmentOfficerResponseDto
                {
                    OfficerID = 1,
                    SSN = "123456789",
                    FName = "Test",
                    LName = "Officer",
                    DateOfBirth = new DateOnly(1990, 1, 1),
                    Email = "test@example.com"
                });

            // Act
            var result = await governmentOfficerService.CreateAsync(dto);

            // Assert
            Assert.Equal(1, result.OfficerID);
            Assert.Equal("test@example.com", result.Email);
            Assert.Equal("Test", result.FName);
            Assert.Equal("Officer", result.LName);

            governmentOfficerRepositoryMock.Verify(r => r.AddAsync(It.IsAny<GovernmentOfficer>()), Times.Once);
            userAccountRepositoryMock.Verify(r => r.AddAsync(It.Is<UserAccount>(account =>
                account.Role == RoleConstants.User)), Times.Once);
            governmentOfficerRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_SuperAdminRoleRequested_ThrowsBadRequestException()
        {
            // Arrange
            var currentUserServiceMock = new Mock<ICurrentUserService>();
            currentUserServiceMock.Setup(x => x.Role)
                .Returns(RoleConstants.SuperAdmin);

            var dto = new CreateGovernmentOfficerDto
            {
                SSN = "123456789",
                FName = "Test",
                LName = "Officer",
                DateOfBirth = new DateOnly(1990, 1, 1),
                Email = "test@example.com",
                Password = "TestPassword123!",
                Role = RoleConstants.SuperAdmin
            };

            var governmentOfficerRepositoryMock = new Mock<IGovernmentOfficerRepository>();
            var userAccountRepositoryMock = new Mock<IUserAccountRepository>();
            var universityRepositoryMock = new Mock<IUniversityRepository>();
            var studentRepositoryMock = new Mock<IStudentRepository>();
            var teacherRepositoryMock = new Mock<ITeacherRepository>();
            var courseRepositoryMock = new Mock<ICourseRepository>();
            var personnelRepositoryMock = new Mock<IPersonnelRepository>();
            var enrollmentRepositoryMock = new Mock<IEnrollmentRepository>();

            var exceptionHelper = new ExceptionHelper(universityRepositoryMock.Object,
                studentRepositoryMock.Object,
                teacherRepositoryMock.Object,
                courseRepositoryMock.Object,
                personnelRepositoryMock.Object,
                enrollmentRepositoryMock.Object);

            var cacheServiceMock = new Mock<IInMemoryCacheService>();
            var mapperMock = new Mock<IMapper>();
            var passwordHasher = new PasswordHasher<UserAccount>();

            var governmentOfficerService = new GovernmentOfficerService(governmentOfficerRepositoryMock.Object,
                userAccountRepositoryMock.Object,
                exceptionHelper,
                cacheServiceMock.Object,
                mapperMock.Object,
                passwordHasher,
                currentUserServiceMock.Object);

            //Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(() => governmentOfficerService.CreateAsync(dto));
            // Assert
            Assert.Equal("Invalid role. Allowed roles are User, Manager, or Admin.", exception.Message);

            governmentOfficerRepositoryMock.Verify(r => r.AddAsync(It.IsAny<GovernmentOfficer>()), Times.Never);
            userAccountRepositoryMock.Verify(r => r.AddAsync(It.IsAny<UserAccount>()), Times.Never);
            governmentOfficerRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }
}