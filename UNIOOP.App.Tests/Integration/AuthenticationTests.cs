using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using UNIOOP.App.Dtos.Auth;
using UNIOOP.App.Tests.Integration.Infrastructure;

namespace UNIOOP.App.Tests.Integration
{
    public class AuthenticationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public AuthenticationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "superadmin@test.local",
                Password = "IntegrationTestPassword123!"
            };

            // Act
            var response =
                await _client.PostAsJsonAsync("/api/Auth/login", loginDto);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var loginResponse =
                await response.Content.ReadFromJsonAsync<LoginResponseDto>();

            Assert.NotNull(loginResponse);
            Assert.False(
                string.IsNullOrWhiteSpace(loginResponse.AccessToken));
        }

        [Fact]
        public async Task Login_InvalidPassword_ReturnsBadRequest()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "superadmin@test.local",
                Password = "WrongPassword123!"
            };

            // Act
            var response =
                await _client.PostAsJsonAsync("/api/Auth/login", loginDto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Login_InvalidEmail_ReturnsBadRequest()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "doesnotexist@example.com",
                Password = "IntegrationTestPassword123!"
            };

            // Act
            var response =
                await _client.PostAsJsonAsync("/api/Auth/login", loginDto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Login_ValidCredentials_TokenCanAccessProtectedEndpoint()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "superadmin@test.local",
                Password = "IntegrationTestPassword123!"
            };

            var loginResponse =
                await _client.PostAsJsonAsync("/api/Auth/login", loginDto);

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var tokenResponse =
                await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

            Assert.NotNull(tokenResponse);
            Assert.False(
                string.IsNullOrWhiteSpace(tokenResponse.AccessToken));

            // Create an authenticated request
            using var request =
                new HttpRequestMessage(HttpMethod.Get, "/University/GetAll");

            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    tokenResponse.AccessToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
