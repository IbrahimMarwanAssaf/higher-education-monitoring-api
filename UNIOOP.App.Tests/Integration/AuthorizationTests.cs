using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UNIOOP.App.Data;
using UNIOOP.App.Dtos.Auth;
using UNIOOP.App.Dtos.GovernmentOfficers;
using UNIOOP.App.Dtos.Universities;
using UNIOOP.App.Models;
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
            var loginDto = new LoginDto
            {
                Email = "user@test.local",
                Password = _configuration["IntegrationTestUsers:Password"]!
            };

            var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", loginDto);

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var tokenResponse = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

            Assert.NotNull(tokenResponse);
            Assert.False(string.IsNullOrWhiteSpace(tokenResponse.AccessToken));

            using var request = new HttpRequestMessage(HttpMethod.Delete, "/University/Delete/1");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

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

            var loginDto = new LoginDto
            {
                Email = "admin@test.local",
                Password = _configuration["IntegrationTestUsers:Password"]!
            };

            var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", loginDto);

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var tokenResponse = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

            Assert.NotNull(tokenResponse);
            Assert.False(string.IsNullOrWhiteSpace(tokenResponse.AccessToken));

            using var request = new HttpRequestMessage(HttpMethod.Delete, $"/University/Delete/{universityId}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task AdminEndpoint_ManagerRole_ReturnsForbidden()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "manager@test.local",
                Password = _configuration["IntegrationTestUsers:Password"]!
            };

            var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", loginDto);

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var tokenResponse = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

            Assert.NotNull(tokenResponse);
            Assert.False(string.IsNullOrWhiteSpace(tokenResponse.AccessToken));

            using var request = new HttpRequestMessage(HttpMethod.Delete, "/University/Delete/1");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

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

            var loginDto = new LoginDto
            {
                Email = "superadmin@test.local",
                Password = _configuration["IntegrationTestUsers:Password"]!
            };

            var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", loginDto);

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var tokenResponse = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

            Assert.NotNull(tokenResponse);
            Assert.False(string.IsNullOrWhiteSpace(tokenResponse.AccessToken));

            using var request = new HttpRequestMessage(HttpMethod.Delete, $"/University/Delete/{universityId}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task ManagerEndpoint_UserRole_ReturnsForbidden()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "user@test.local",
                Password = _configuration["IntegrationTestUsers:Password"]!
            };

            var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", loginDto);

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var tokenResponse = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

            Assert.NotNull(tokenResponse);
            Assert.False(string.IsNullOrWhiteSpace(tokenResponse.AccessToken));

            using var request = new HttpRequestMessage(HttpMethod.Post, "/University/Create");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

            request.Content = JsonContent.Create(new UniversityCreateUpdateDto
            {
                UniversityName = "Unauthorized Test University"
            });

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task ManagerEndpoint_ManagerRole_IsAllowed()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "manager@test.local",
                Password = _configuration["IntegrationTestUsers:Password"]!
            };

            var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", loginDto);

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var tokenResponse = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

            Assert.NotNull(tokenResponse);
            Assert.False(string.IsNullOrWhiteSpace(tokenResponse.AccessToken));

            using var request = new HttpRequestMessage(HttpMethod.Post, "/University/Create");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

            request.Content = JsonContent.Create(new UniversityCreateUpdateDto
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
            var loginDto = new LoginDto
            {
                Email = "admin@test.local",
                Password = _configuration["IntegrationTestUsers:Password"]!
            };

            var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", loginDto);

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var tokenResponse = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

            Assert.NotNull(tokenResponse);
            Assert.False(string.IsNullOrWhiteSpace(tokenResponse.AccessToken));

            using var request = new HttpRequestMessage(HttpMethod.Post, "/University/Create");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

            request.Content = JsonContent.Create(new UniversityCreateUpdateDto
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
            var loginDto = new LoginDto
            {
                Email = "superadmin@test.local",
                Password = _configuration["IntegrationTestUsers:Password"]!
            };

            var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", loginDto);

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var tokenResponse = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

            Assert.NotNull(tokenResponse);
            Assert.False(string.IsNullOrWhiteSpace(tokenResponse.AccessToken));

            using var request = new HttpRequestMessage(HttpMethod.Post, "/University/Create");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

            request.Content = JsonContent.Create(new UniversityCreateUpdateDto
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
            var loginDto = new LoginDto
            {
                Email = "admin@test.local",
                Password = _configuration["IntegrationTestUsers:Password"]!
            };

            var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", loginDto);

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var tokenResponse = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

            Assert.NotNull(tokenResponse);
            Assert.False(string.IsNullOrWhiteSpace(tokenResponse.AccessToken));

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

            using var request = new HttpRequestMessage(HttpMethod.Post, "/GovernmentOfficers/Create");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

            request.Content = JsonContent.Create(createDto);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateGovernmentOfficer_SuperAdminRole_AdminRequestingAdminRole_ReturnsCreated()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "superadmin@test.local",
                Password = _configuration["IntegrationTestUsers:Password"]!
            };

            var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", loginDto);

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var tokenResponse = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

            Assert.NotNull(tokenResponse);
            Assert.False(string.IsNullOrWhiteSpace(tokenResponse.AccessToken));

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

            using var request = new HttpRequestMessage(HttpMethod.Post, "/GovernmentOfficers/Create");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

            request.Content = JsonContent.Create(createDto);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task CreateGovernmentOfficer_AdminRole_UserRole_ReturnsCreated()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "admin@test.local",
                Password = _configuration["IntegrationTestUsers:Password"]!
            };

            var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", loginDto);

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var tokenResponse = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

            Assert.NotNull(tokenResponse);
            Assert.False(string.IsNullOrWhiteSpace(tokenResponse.AccessToken));

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

            using var request = new HttpRequestMessage(HttpMethod.Post, "/GovernmentOfficers/Create");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

            request.Content = JsonContent.Create(createDto);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task CreateGovernmentOfficer_AdminRole_ManagerRole_ReturnsCreated()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "admin@test.local",
                Password = _configuration["IntegrationTestUsers:Password"]!
            };

            var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", loginDto);

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var tokenResponse = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

            Assert.NotNull(tokenResponse);
            Assert.False(string.IsNullOrWhiteSpace(tokenResponse.AccessToken));

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

            using var request = new HttpRequestMessage(HttpMethod.Post, "/GovernmentOfficers/Create");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

            request.Content = JsonContent.Create(createDto);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
    }
}