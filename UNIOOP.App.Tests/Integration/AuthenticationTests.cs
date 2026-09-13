using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UNIOOP.App.Tests.Integration.Helpers;
using UNIOOP.App.Tests.Integration.Infrastructure;

namespace UNIOOP.App.Tests.Integration
{
    [Collection("Integration Tests")]
    public class AuthenticationTests
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;

        public AuthenticationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            _configuration = factory.Services.GetRequiredService<IConfiguration>();
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var token = await AuthenticationHelper.GetTokenAsync(_client, "superadmin@test.local",
                _configuration["IntegrationTestUsers:Password"]!);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(token));
        }

        [Fact]
        public async Task Login_InvalidPassword_ReturnsBadRequest()
        {
            // Arrange
            var loginDto = new Dtos.Auth.LoginDto
            {
                Email = "superadmin@test.local",
                Password = "WrongPassword123!"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Auth/login", loginDto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Login_InvalidEmail_ReturnsBadRequest()
        {
            // Arrange
            var loginDto = new Dtos.Auth.LoginDto
            {
                Email = "doesnotexist@example.com",
                Password = _configuration["IntegrationTestUsers:Password"]!
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Auth/login", loginDto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Login_ValidCredentials_TokenCanAccessProtectedEndpoint()
        {
            // Arrange
            var token = await AuthenticationHelper.GetTokenAsync(_client, "superadmin@test.local",
                _configuration["IntegrationTestUsers:Password"]!);

            using var request = HttpRequestHelper.CreateAuthenticatedRequest(HttpMethod.Get, "/University/GetAll", token);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
