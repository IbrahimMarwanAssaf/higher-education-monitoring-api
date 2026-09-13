using Microsoft.AspNetCore.Identity;
using Moq;
using UNIOOP.App.Dtos.Auth;
using UNIOOP.App.Exceptions;
using UNIOOP.App.Helpers;
using UNIOOP.App.Models;
using UNIOOP.App.Repositories.Interfaces;
using UNIOOP.App.Services;
using UNIOOP.App.Services.Interfaces;
using Xunit;

namespace UNIOOP.App.Tests.Unit
{
    public class AuthServiceTests
    {
        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var repositoryMock = new Mock<IUserAccountRepository>();

            var userAccount = new UserAccount
            {
                PersonnelID = 1,
                Role = "SuperAdmin"
            };

            var passwordHasher = new PasswordHasher<UserAccount>();
            userAccount.PasswordHash = passwordHasher.HashPassword(userAccount, "TestPassword123!");

            repositoryMock.Setup(r => r.GetByEmailAsync("test@example.com"))
                .ReturnsAsync(userAccount);

            var tokenServiceMock = new Mock<ITokenService>();
            tokenServiceMock.Setup(t => t.GenerateAccessToken(1, "SuperAdmin"))
                .Returns("fake-jwt-token");

            var universityRepositoryMock = new Mock<IUniversityRepository>();
            var studentRepositoryMock = new Mock<IStudentRepository>();
            var teacherRepositoryMock = new Mock<ITeacherRepository>();
            var courseRepositoryMock = new Mock<ICourseRepository>();
            var personnelRepositoryMock = new Mock<IPersonnelRepository>();
            var enrollmentRepositoryMock = new Mock<IEnrollmentRepository>();

            var exceptionHelper = new ExceptionHelper(
                universityRepositoryMock.Object,
                studentRepositoryMock.Object,
                teacherRepositoryMock.Object,
                courseRepositoryMock.Object,
                personnelRepositoryMock.Object,
                enrollmentRepositoryMock.Object);

            var authService = new AuthService(repositoryMock.Object,
                exceptionHelper,
                passwordHasher,
                tokenServiceMock.Object);

            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "TestPassword123!"
            };

            // Act
            var result = await authService.LoginAsync(loginDto);

            // Assert
            Assert.Equal("fake-jwt-token", result.AccessToken);

            tokenServiceMock.Verify(t => t.GenerateAccessToken(1, "SuperAdmin"), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_InvalidEmail_ThrowsBadRequestException()
        {
            // Arrange
            var repositoryMock = new Mock<IUserAccountRepository>();

            repositoryMock.Setup(r => r.GetByEmailAsync("unknown@example.com"))
                .ReturnsAsync((UserAccount?)null);

            var universityRepositoryMock = new Mock<IUniversityRepository>();
            var studentRepositoryMock = new Mock<IStudentRepository>();
            var teacherRepositoryMock = new Mock<ITeacherRepository>();
            var courseRepositoryMock = new Mock<ICourseRepository>();
            var personnelRepositoryMock = new Mock<IPersonnelRepository>();
            var enrollmentRepositoryMock = new Mock<IEnrollmentRepository>();

            var exceptionHelper = new ExceptionHelper(
                universityRepositoryMock.Object,
                studentRepositoryMock.Object,
                teacherRepositoryMock.Object,
                courseRepositoryMock.Object,
                personnelRepositoryMock.Object,
                enrollmentRepositoryMock.Object);

            var passwordHasher = new PasswordHasher<UserAccount>();
            var tokenServiceMock = new Mock<ITokenService>();

            var authService = new AuthService(
                repositoryMock.Object,
                exceptionHelper,
                passwordHasher,
                tokenServiceMock.Object);

            var loginDto = new LoginDto
            {
                Email = "unknown@example.com",
                Password = "TestPassword123!"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(() => authService.LoginAsync(loginDto));
            // Assert
            Assert.Equal("Invalid email or password.", exception.Message);

            tokenServiceMock.Verify(t => t.GenerateAccessToken(It.IsAny<long>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_InvalidPassword_ThrowsBadRequestException()
        {
            // Arrange
            var repositoryMock = new Mock<IUserAccountRepository>();

            var userAccount = new UserAccount
            {
                PersonnelID = 1,
                Role = "SuperAdmin"
            };

            var passwordHasher = new PasswordHasher<UserAccount>();
            userAccount.PasswordHash = passwordHasher.HashPassword(userAccount, "CorrectPassword123!");

            repositoryMock.Setup(r => r.GetByEmailAsync("test@example.com"))
                .ReturnsAsync(userAccount);

            var universityRepositoryMock = new Mock<IUniversityRepository>();
            var studentRepositoryMock = new Mock<IStudentRepository>();
            var teacherRepositoryMock = new Mock<ITeacherRepository>();
            var courseRepositoryMock = new Mock<ICourseRepository>();
            var personnelRepositoryMock = new Mock<IPersonnelRepository>();
            var enrollmentRepositoryMock = new Mock<IEnrollmentRepository>();

            var exceptionHelper = new ExceptionHelper(
                universityRepositoryMock.Object,
                studentRepositoryMock.Object,
                teacherRepositoryMock.Object,
                courseRepositoryMock.Object,
                personnelRepositoryMock.Object,
                enrollmentRepositoryMock.Object);

            var tokenServiceMock = new Mock<ITokenService>();

            var authService = new AuthService(
                repositoryMock.Object,
                exceptionHelper,
                passwordHasher,
                tokenServiceMock.Object);

            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "WrongPassword123!"
            };

            // Act + Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(() => authService.LoginAsync(loginDto));
            // Assert
            Assert.Equal("Invalid email or password.", exception.Message);

            tokenServiceMock.Verify(t => t.GenerateAccessToken(It.IsAny<long>(), It.IsAny<string>()), Times.Never);
        }
    }
}