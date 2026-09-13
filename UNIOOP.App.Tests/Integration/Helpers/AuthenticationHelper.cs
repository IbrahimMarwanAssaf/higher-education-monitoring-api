using System.Net;
using System.Net.Http.Json;
using UNIOOP.App.Dtos.Auth;

namespace UNIOOP.App.Tests.Integration.Helpers
{
    public static class AuthenticationHelper
    {
        public static async Task<string> GetTokenAsync(HttpClient client, string email, string password)
        {
            var loginDto = new LoginDto
            {
                Email = email,
                Password = password
            };

            var response = await client.PostAsJsonAsync("/api/Auth/login", loginDto);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new InvalidOperationException(
                    $"Login failed for {email}. " +
                    $"Expected 200 but received {(int)response.StatusCode}.");
            }

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

            if (loginResponse == null || string.IsNullOrWhiteSpace(loginResponse.AccessToken))
            {
                throw new InvalidOperationException(
                    $"Login succeeded for {email}, but no access token was returned.");
            }

            return loginResponse.AccessToken;
        }
    }
}