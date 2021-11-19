using R.Systems.Auth.FunctionalTests.Initializers;
using R.Systems.Auth.FunctionalTests.Models;
using R.Systems.Auth.WebApi.Features.Authentication;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace R.Systems.Auth.FunctionalTests
{
    public class UserControllerTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _httpClient;

        public UserControllerTests(CustomWebApplicationFactory<Startup> webApplicationFactory)
        {
            _httpClient = webApplicationFactory.CreateClient();
        }

        [Theory]
        [InlineData("test32@lukaszrydzkowski.pl")]
        public async Task Login_WrongEmail_Unauthorized(string email)
        {
            HttpStatusCode expectedHttpStatusCode = HttpStatusCode.Unauthorized;
            AuthenticateRequest request = new()
            {
                Email = email,
                Password = new Users().Data[0].Password
            };
            var requestContent = new StringContent(
                JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"
            );

            HttpResponseMessage httpResponse = await _httpClient.PostAsync("/users/authenticate", requestContent);

            Assert.Equal(expectedHttpStatusCode, httpResponse.StatusCode);
        }

        [Theory]
        [InlineData("1231231")]
        public async Task Login_WrongPassword_Unauthorized(string password)
        {
            HttpStatusCode expectedHttpStatusCode = HttpStatusCode.Unauthorized;
            AuthenticateRequest request = new()
            {
                Email = new Users().Data[1].Password,
                Password = password
            };
            var requestContent = new StringContent(
                JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"
            );

            HttpResponseMessage httpResponse = await _httpClient.PostAsync("/users/authenticate", requestContent);

            Assert.Equal(expectedHttpStatusCode, httpResponse.StatusCode);
        }

        [Theory]
        [MemberData(nameof(GetLoginCorrectDataParameters))]
        public async Task Login_CorrectData_Authorized(string email, string password)
        {
            JsonSerializerOptions jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
            HttpStatusCode expectedHttpStatusCode = HttpStatusCode.OK;
            AuthenticateRequest request = new()
            {
                Email = email,
                Password = password
            };
            var requestContent = new StringContent(
                JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"
            );

            HttpResponseMessage httpResponse = await _httpClient.PostAsync("/users/authenticate", requestContent);
            string responseContent = await httpResponse.Content.ReadAsStringAsync();

            Assert.Equal(expectedHttpStatusCode, httpResponse.StatusCode);
            AuthenticateResponse? response = JsonSerializer.Deserialize<AuthenticateResponse>(
                responseContent, jsonSerializerOptions
            );
            Assert.NotNull(response);
        }

        public static IEnumerable<object[]> GetLoginCorrectDataParameters()
        {
            Users users = new();
            return new List<object[]>
            {
                new object[] { users.Data[0].Email, users.Data[0].Password },
                new object[] { users.Data[1].Email, "" },
                new object[] { $" {users.Data[1].Email} ", $" {users.Data[1].Password} " }
            };
        }
    }
}
