using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using R.Systems.Auth.FunctionalTests.Initializers;
using R.Systems.Auth.WebApi;
using R.Systems.Auth.WebApi.Features.Version;
using Xunit;

namespace R.Systems.Auth.FunctionalTests.Tests.VersionTests;

public class GetVersionTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _httpClient;

    public GetVersionTests(CustomWebApplicationFactory<Program> webApplicationFactory)
    {
        _httpClient = webApplicationFactory.CreateClient();
    }

    [Fact]
    public async Task GetVersion_CorrectData_ReturnsCorrectVersion()
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
        Assert.StartsWith(expectedResponse.Version, parsedResponse?.Version ?? "");
    }
}
