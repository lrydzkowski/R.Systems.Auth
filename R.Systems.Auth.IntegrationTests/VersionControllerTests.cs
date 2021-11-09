using R.Systems.Auth.IntegrationTests.Initializers;
using R.Systems.Auth.WebApi.Models;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace R.Systems.Auth.IntegrationTests
{
    public class VersionControllerTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _httpClient;

        public VersionControllerTests(CustomWebApplicationFactory<Startup> webApplicationFactory)
        {
            _httpClient = webApplicationFactory.CreateClient();
        }

        [Fact]
        public async Task SendGetVersionRequest_EndpointReturnsCorrectVersion()
        {
            VersionResponse expectedResponse = new()
            {
                Version = "1.0.0"
            };
            JsonSerializerOptions jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

            HttpResponseMessage httpResponse = await _httpClient.GetAsync("/version");

            httpResponse.EnsureSuccessStatusCode();
            string responseContent = await httpResponse.Content.ReadAsStringAsync();
            VersionResponse? parsedResponse = JsonSerializer.Deserialize<VersionResponse>(
                responseContent, jsonSerializerOptions
            );
            Assert.Equal(expectedResponse.Version, parsedResponse?.Version);
        }
    }
}
