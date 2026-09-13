using Moq;
using UNIOOP.App.Constants;
using UNIOOP.App.Exceptions;
using UNIOOP.App.Models;
using UNIOOP.App.Tests.Unit.Helpers;

namespace UNIOOP.App.Tests.Unit
{
    public class GovernmentOfficerServiceTests
    {
        [Fact]
        public async Task CreateAsync_AdminCreatingAdmin_ThrowsBadRequestException()
        {
            // Arrange
            var helper = new GovernmentOfficerServiceTestHelper(RoleConstants.Admin);

            var dto = helper.CreateDto(RoleConstants.Admin);
            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(() => helper.Service.CreateAsync(dto));

            Assert.Equal("An Admin cannot create another Admin.", exception.Message);

            helper.GovernmentOfficerRepositoryMock.Verify(r => r.AddAsync(It.IsAny<GovernmentOfficer>()), Times.Never);
            helper.UserAccountRepositoryMock.Verify(r => r.AddAsync(It.IsAny<UserAccount>()), Times.Never);
            helper.GovernmentOfficerRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_SuperAdminCreatingAdmin_CreatesOfficer()
        {
            // Arrange
            var helper = new GovernmentOfficerServiceTestHelper(RoleConstants.SuperAdmin);

            var dto = helper.CreateDto(RoleConstants.Admin);

            helper.SetupSuccessfulCreate();
            // Act
            var result = await helper.Service.CreateAsync(dto);
            // Assert
            Assert.Equal(1, result.OfficerID);
            Assert.Equal("test@example.com", result.Email);
            Assert.Equal("Test", result.FName);
            Assert.Equal("Officer", result.LName);

            helper.GovernmentOfficerRepositoryMock.Verify(r => r.AddAsync(It.IsAny<GovernmentOfficer>()), Times.Once);
            helper.UserAccountRepositoryMock.Verify(r => r.AddAsync(It.Is<UserAccount>(account =>
                 account.Role == RoleConstants.Admin)), Times.Once);
            helper.GovernmentOfficerRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_AdminCreatingManager_CreatesOfficer()
        {
            // Arrange
            var helper = new GovernmentOfficerServiceTestHelper(RoleConstants.Admin);

            var dto = helper.CreateDto(RoleConstants.Manager);

            helper.SetupSuccessfulCreate();
            // Act
            var result = await helper.Service.CreateAsync(dto);
            // Assert
            Assert.Equal(1, result.OfficerID);
            Assert.Equal("test@example.com", result.Email);
            Assert.Equal("Test", result.FName);
            Assert.Equal("Officer", result.LName);

            helper.GovernmentOfficerRepositoryMock.Verify(r => r.AddAsync(It.IsAny<GovernmentOfficer>()), Times.Once);
            helper.UserAccountRepositoryMock.Verify(r => r.AddAsync(It.Is<UserAccount>(account => account.Role == RoleConstants.Manager)), Times.Once);
            helper.GovernmentOfficerRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_AdminCreatingUser_CreatesOfficer()
        {
            // Arrange
            var helper = new GovernmentOfficerServiceTestHelper(RoleConstants.Admin);

            var dto = helper.CreateDto(RoleConstants.User);

            helper.SetupSuccessfulCreate();
            // Act
            var result = await helper.Service.CreateAsync(dto);
            // Assert
            Assert.Equal(1, result.OfficerID);
            Assert.Equal("test@example.com", result.Email);
            Assert.Equal("Test", result.FName);
            Assert.Equal("Officer", result.LName);
            helper.GovernmentOfficerRepositoryMock.Verify(r => r.AddAsync(It.IsAny<GovernmentOfficer>()), Times.Once);
            helper.UserAccountRepositoryMock.Verify(r => r.AddAsync(It.Is<UserAccount>(account =>
                account.Role == RoleConstants.User)), Times.Once);
            helper.GovernmentOfficerRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_SuperAdminRoleRequested_ThrowsBadRequestException()
        {
            // Arrange
            var helper = new GovernmentOfficerServiceTestHelper(RoleConstants.SuperAdmin);
            var dto = helper.CreateDto(RoleConstants.SuperAdmin);
            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(() => helper.Service.CreateAsync(dto));

            Assert.Equal("Invalid role. Allowed roles are User, Manager, or Admin.", exception.Message);

            helper.GovernmentOfficerRepositoryMock.Verify(r => r.AddAsync(It.IsAny<GovernmentOfficer>()), Times.Never);
            helper.UserAccountRepositoryMock.Verify(r => r.AddAsync(It.IsAny<UserAccount>()), Times.Never);
            helper.GovernmentOfficerRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }
}
