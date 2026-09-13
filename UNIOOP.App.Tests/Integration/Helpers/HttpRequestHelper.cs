using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace UNIOOP.App.Tests.Integration.Helpers
{
    public static class HttpRequestHelper
    {
        public static HttpRequestMessage CreateAuthenticatedRequest(HttpMethod method,
            string url,
            string token,
            object? content = null)
        {
            var request = new HttpRequestMessage(method, url);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (content != null)
            {
                request.Content = JsonContent.Create(content);
            }

            return request;
        }
    }
}