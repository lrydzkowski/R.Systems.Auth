using Microsoft.AspNetCore.Mvc.Testing;
using R.Systems.Auth.Models;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace R.Systems.Auth.IntegrationTests
{
    public class VersionControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _webApplicationFactory;

        public VersionControllerTests(WebApplicationFactory<Startup> webApplicationFactory)
        {
            _webApplicationFactory = webApplicationFactory;
        }

        [Fact]
        public async Task SendGetVersionRequest_EndpointReturnVersion()
        {
            HttpClient client = _webApplicationFactory.CreateClient();
            VersionResponse expectedResponse = new()
            {
                Version = "1.0.0"
            };
            JsonSerializerOptions jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

            HttpResponseMessage httpResponse = await client.GetAsync("/version");

            httpResponse.EnsureSuccessStatusCode();
            string responseContent = await httpResponse.Content.ReadAsStringAsync();
            VersionResponse parsedResponse = JsonSerializer.Deserialize<VersionResponse>(
                responseContent, jsonSerializerOptions
            );
            Assert.Equal(expectedResponse.Version, parsedResponse.Version);
        }
    }
}
