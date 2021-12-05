using R.Systems.Auth.FunctionalTests.Initializers;
using R.Systems.Auth.WebApi.Features.Version;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace R.Systems.Auth.FunctionalTests
{
    public class GetVersionTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _httpClient;

        public GetVersionTests(CustomWebApplicationFactory<Startup> webApplicationFactory)
        {
            _httpClient = webApplicationFactory.CreateClient();
        }

        [Fact]
        public async Task SendGetVersionRequest_EndpointReturnsCorrectVersion()
        {
            GetVersionResponse expectedResponse = new()
            {
                Version = "1.0.0"
            };
            JsonSerializerOptions jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

            HttpResponseMessage httpResponse = await _httpClient.GetAsync("/version");

            httpResponse.EnsureSuccessStatusCode();
            string responseContent = await httpResponse.Content.ReadAsStringAsync();
            GetVersionResponse? parsedResponse = JsonSerializer.Deserialize<GetVersionResponse>(
                responseContent, jsonSerializerOptions
            );
            Assert.Equal(expectedResponse.Version, parsedResponse?.Version);
        }
    }
}
