using R.Systems.Auth.FunctionalTests.Initializers;
using R.Systems.Auth.FunctionalTests.Models;
using R.Systems.Auth.FunctionalTests.Services;
using R.Systems.Auth.WebApi.Features.Authentication;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace R.Systems.Auth.FunctionalTests
{
    public class AuthenticationControllerTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        public AuthenticationControllerTests(
            CustomWebApplicationFactory<Startup> webApplicationFactory)
        {
            HttpClient = webApplicationFactory.CreateClient();
            RequestService = new RequestService();
        }

        private string AuthenticateUrl { get; } = "/users/authenticate";

        private HttpClient HttpClient { get; }

        private RequestService RequestService { get; }

        [Theory]
        [InlineData("test32@lukaszrydzkowski.pl")]
        public async Task Authenticate_WrongEmail_Unauthorized(string email)
        {
            HttpStatusCode expectedHttpStatusCode = HttpStatusCode.Unauthorized;
            AuthenticateRequest request = new()
            {
                Email = email,
                Password = new Users()[0].Password
            };

            (HttpStatusCode httpStatusCode, _) = await RequestService.SendPostAsync
                <AuthenticateRequest, AuthenticateResponse>(
                    AuthenticateUrl,
                    request,
                    HttpClient
                );

            Assert.Equal(expectedHttpStatusCode, httpStatusCode);
        }

        [Theory]
        [InlineData("1231231")]
        public async Task Authenticate_WrongPassword_Unauthorized(string password)
        {
            HttpStatusCode expectedHttpStatusCode = HttpStatusCode.Unauthorized;
            AuthenticateRequest request = new()
            {
                Email = new Users()[1].Password,
                Password = password
            };

            (HttpStatusCode httpStatusCode, _) = await RequestService.SendPostAsync
                <AuthenticateRequest, AuthenticateResponse>(
                    AuthenticateUrl,
                    request,
                    HttpClient
                );

            Assert.Equal(expectedHttpStatusCode, httpStatusCode);
        }

        [Theory]
        [MemberData(nameof(GetLoginCorrectDataParameters))]
        public async Task Authenticate_CorrectData_GettingJwtTokens(string email, string password)
        {
            HttpStatusCode expectedHttpStatusCode = HttpStatusCode.OK;
            AuthenticateRequest request = new()
            {
                Email = email,
                Password = password
            };

            (HttpStatusCode httpStatusCode, AuthenticateResponse? response) = await RequestService.SendPostAsync
                <AuthenticateRequest, AuthenticateResponse>(
                    AuthenticateUrl,
                    request,
                    HttpClient
                );

            Assert.Equal(expectedHttpStatusCode, httpStatusCode);
            Assert.NotNull(response);
            Assert.False(string.IsNullOrEmpty(response?.AccessToken));
            Assert.False(string.IsNullOrEmpty(response?.RefreshToken));
        }

        public static IEnumerable<object[]> GetLoginCorrectDataParameters()
        {
            Users users = new();
            return new List<object[]>
            {
                new object[] { users[0].Email, users[0].Password },
                new object[] { users[1].Email, "" },
                new object[] { $" {users[1].Email} ", $" {users[1].Password} " }
            };
        }
    }
}
