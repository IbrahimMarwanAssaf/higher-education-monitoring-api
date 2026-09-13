using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UNIOOP.App.Data;
using UNIOOP.App.Dtos.GovernmentOfficers;
using UNIOOP.App.Dtos.Universities;
using UNIOOP.App.Models;
using UNIOOP.App.Tests.Integration.Helpers;
using UNIOOP.App.Tests.Integration.Infrastructure;

namespace UNIOOP.App.Tests.Integration
{
    [Collection("Integration Tests")]
    public class AuthorizationTests
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        private readonly CustomWebApplicationFactory _factory;

        public AuthorizationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _configuration = factory.Services.GetRequiredService<IConfiguration>();
        }

        private async Task<int> CreateTestUniversityAsync()
        {
            using var scope = _factory.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<DataContextEF>();

            var university = new University
            {
                UniversityName = $"Test University {Guid.NewGuid():N}"
            };

            context.Universities.Add(university);

            await context.SaveChangesAsync();

            return university.UniversityID;
        }

        [Fact]
        public async Task GetAllUniversities_WithoutAuthentication_ReturnsUnauthorized()
        {
            // Act
            var response = await _client.GetAsync("/University/GetAll");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task AdminEndpoint_UserRole_ReturnsForbidden()
        {
            // Arrange
            var token = await AuthenticationHelper.GetTokenAsync(_client, "user@test.local",
                _configuration["IntegrationTestUsers:Password"]!);

            using var request = HttpRequestHelper.CreateAuthenticatedRequest(
                HttpMethod.Delete,
                "/University/Delete/1",
                token);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task AdminEndpoint_AdminRole_IsAllowed()
        {
            // Arrange
            var universityId = await CreateTestUniversityAsync();

            var token = await AuthenticationHelper.GetTokenAsync(_client, "admin@test.local",
                _configuration["IntegrationTestUsers:Password"]!);

            using var request = HttpRequestHelper.CreateAuthenticatedRequest(
                HttpMethod.Delete,
                $"/University/Delete/{universityId}",
                token);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task AdminEndpoint_ManagerRole_ReturnsForbidden()
        {
            // Arrange
            var token = await AuthenticationHelper.GetTokenAsync(_client, "manager@test.local",
                _configuration["IntegrationTestUsers:Password"]!);

            using var request = HttpRequestHelper.CreateAuthenticatedRequest(
                HttpMethod.Delete,
                "/University/Delete/1",
                token);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task AdminEndpoint_SuperAdminRole_IsAllowed()
        {
            // Arrange
            var universityId = await CreateTestUniversityAsync();

            var token = await AuthenticationHelper.GetTokenAsync(_client, "superadmin@test.local",
                _configuration["IntegrationTestUsers:Password"]!);

            using var request = HttpRequestHelper.CreateAuthenticatedRequest(
                HttpMethod.Delete,
                $"/University/Delete/{universityId}",
                token);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task ManagerEndpoint_UserRole_ReturnsForbidden()
        {
            // Arrange
            var token = await AuthenticationHelper.GetTokenAsync(_client, "user@test.local",
                _configuration["IntegrationTestUsers:Password"]!);

            using var request = HttpRequestHelper.CreateAuthenticatedRequest(
                HttpMethod.Post,
                "/University/Create",
                token,
                new UniversityCreateUpdateDto
                {
                    UniversityName = "Unauthorized Test University"
                });

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(
                HttpStatusCode.Forbidden,
                response.StatusCode);
        }

        [Fact]
        public async Task ManagerEndpoint_ManagerRole_IsAllowed()
        {
            // Arrange
            var token = await AuthenticationHelper.GetTokenAsync(_client, "manager@test.local",
                _configuration["IntegrationTestUsers:Password"]!);

            using var request = HttpRequestHelper.CreateAuthenticatedRequest(
                HttpMethod.Post,
                "/University/Create",
                token,
                new UniversityCreateUpdateDto
                {
                    UniversityName = $"Manager Test University {Guid.NewGuid():N}"
                });

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task ManagerEndpoint_AdminRole_IsAllowed()
        {
            // Arrange
            var token = await AuthenticationHelper.GetTokenAsync(_client, "admin@test.local",
                _configuration["IntegrationTestUsers:Password"]!);

            using var request = HttpRequestHelper.CreateAuthenticatedRequest(
                HttpMethod.Post,
                "/University/Create",
                token,
                new UniversityCreateUpdateDto
                {
                    UniversityName = $"Admin Test University {Guid.NewGuid():N}"
                });

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task ManagerEndpoint_SuperAdminRole_IsAllowed()
        {
            // Arrange
            var token = await AuthenticationHelper.GetTokenAsync(_client, "superadmin@test.local",
                _configuration["IntegrationTestUsers:Password"]!);

            using var request = HttpRequestHelper.CreateAuthenticatedRequest(
                HttpMethod.Post,
                "/University/Create",
                token,
                new UniversityCreateUpdateDto
                {
                    UniversityName = $"SuperAdmin Test University {Guid.NewGuid():N}"
                });

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task CreateGovernmentOfficer_AdminRole_AdminRequestingAdminRole_ReturnsBadRequest()
        {
            // Arrange
            var token = await AuthenticationHelper.GetTokenAsync(_client, "admin@test.local",
                _configuration["IntegrationTestUsers:Password"]!);

            var createDto = new CreateGovernmentOfficerDto
            {
                SSN = $"TEST-NEW-{Guid.NewGuid():N}"[..20],
                FName = "Test",
                LName = "UnauthorizedAdmin",
                DateOfBirth = new DateOnly(1990, 5, 1),
                Email = $"newadmin-{Guid.NewGuid():N}@test.local",
                Password = "NewAdminPassword123!",
                Role = "Admin"
            };

            using var request = HttpRequestHelper.CreateAuthenticatedRequest(
                HttpMethod.Post,
                "/GovernmentOfficers/Create",
                token,
                createDto);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateGovernmentOfficer_SuperAdminRole_AdminRequestingAdminRole_ReturnsCreated()
        {
            // Arrange
            var token = await AuthenticationHelper.GetTokenAsync(_client, "superadmin@test.local",
                _configuration["IntegrationTestUsers:Password"]!);

            var uniqueId = Guid.NewGuid().ToString("N");

            var createDto = new CreateGovernmentOfficerDto
            {
                SSN = $"TEST-{uniqueId}"[..20],
                FName = "Test",
                LName = "NewAdmin",
                DateOfBirth = new DateOnly(1990, 5, 1),
                Email = $"newadmin-{uniqueId}@test.local",
                Password = "NewAdminPassword123!",
                Role = "Admin"
            };

            using var request = HttpRequestHelper.CreateAuthenticatedRequest(
                HttpMethod.Post,
                "/GovernmentOfficers/Create",
                token,
                createDto);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task CreateGovernmentOfficer_AdminRole_UserRole_ReturnsCreated()
        {
            // Arrange
            var token = await AuthenticationHelper.GetTokenAsync(_client, "admin@test.local",
                _configuration["IntegrationTestUsers:Password"]!);

            var uniqueId = Guid.NewGuid().ToString("N");

            var createDto = new CreateGovernmentOfficerDto
            {
                SSN = $"TEST-{uniqueId}"[..20],
                FName = "Test",
                LName = "NewUser",
                DateOfBirth = new DateOnly(1990, 6, 1),
                Email = $"newuser-{uniqueId}@test.local",
                Password = "NewUserPassword123!",
                Role = "User"
            };

            using var request = HttpRequestHelper.CreateAuthenticatedRequest(
                HttpMethod.Post,
                "/GovernmentOfficers/Create",
                token,
                createDto);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task CreateGovernmentOfficer_AdminRole_ManagerRole_ReturnsCreated()
        {
            // Arrange
            var token = await AuthenticationHelper.GetTokenAsync(_client, "admin@test.local",
                _configuration["IntegrationTestUsers:Password"]!);

            var uniqueId = Guid.NewGuid().ToString("N");

            var createDto = new CreateGovernmentOfficerDto
            {
                SSN = $"TEST-{uniqueId}"[..20],
                FName = "Test",
                LName = "NewManager",
                DateOfBirth = new DateOnly(1990, 6, 1),
                Email = $"newmanager-{uniqueId}@test.local",
                Password = "NewManagerPassword123!",
                Role = "Manager"
            };

            using var request = HttpRequestHelper.CreateAuthenticatedRequest(
                HttpMethod.Post,
                "/GovernmentOfficers/Create",
                token,
                createDto);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
    }
}