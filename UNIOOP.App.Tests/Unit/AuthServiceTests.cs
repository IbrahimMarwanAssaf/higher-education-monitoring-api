using Moq;
using UNIOOP.App.Dtos.Auth;
using UNIOOP.App.Exceptions;
using UNIOOP.App.Models;
using UNIOOP.App.Tests.Unit.Helpers;

namespace UNIOOP.App.Tests.Unit
{
    public class AuthServiceTests
    {
        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var helper = new AuthServiceTestHelper();

            var userAccount = helper.CreateUserAccount();

            helper.RepositoryMock.Setup(r => r.GetByEmailAsync("test@example.com"))
                .ReturnsAsync(userAccount);

            helper.TokenServiceMock.Setup(t => t.GenerateAccessToken(1, "SuperAdmin"))
                .Returns("fake-jwt-token");

            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "TestPassword123!"
            };

            // Act
            var result = await helper.Service.LoginAsync(loginDto);

            // Assert
            Assert.Equal("fake-jwt-token", result.AccessToken);
            helper.TokenServiceMock.Verify(t => t.GenerateAccessToken(1, "SuperAdmin"), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_InvalidEmail_ThrowsBadRequestException()
        {
            // Arrange
            var helper = new AuthServiceTestHelper();
            helper.RepositoryMock.Setup(r => r.GetByEmailAsync("unknown@example.com"))
                .ReturnsAsync((UserAccount)null!);

            var loginDto = new LoginDto
            {
                Email = "unknown@example.com",
                Password = "TestPassword123!"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(
                () => helper.Service.LoginAsync(loginDto));

            Assert.Equal("Invalid email or password.", exception.Message);
            helper.TokenServiceMock.Verify(t => t.GenerateAccessToken(It.IsAny<long>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_InvalidPassword_ThrowsBadRequestException()
        {
            // Arrange
            var helper = new AuthServiceTestHelper();

            var userAccount = helper.CreateUserAccount(password: "CorrectPassword123!");

            helper.RepositoryMock.Setup(r => r.GetByEmailAsync("test@example.com"))
                .ReturnsAsync(userAccount);

            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "WrongPassword123!"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(() => helper.Service.LoginAsync(loginDto));

            Assert.Equal("Invalid email or password.", exception.Message);

            helper.TokenServiceMock.Verify(t => t.GenerateAccessToken(It.IsAny<long>(), It.IsAny<string>()), Times.Never);
        }
    }
}
