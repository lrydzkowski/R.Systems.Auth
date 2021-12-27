using R.Systems.Auth.Core.Models;
using R.Systems.Auth.FunctionalTests.Initializers;
using R.Systems.Auth.FunctionalTests.Models;
using R.Systems.Auth.FunctionalTests.Services;
using R.Systems.Auth.WebApi;
using R.Systems.Auth.WebApi.Features.Authentication;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace R.Systems.Auth.FunctionalTests.Tests.UserTests
{
    public class GetUsersTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        public GetUsersTests(CustomWebApplicationFactory<Program> webApplicationFactory)
        {
            HttpClient = webApplicationFactory.CreateClient();
            RequestService = new RequestService();
            Authenticator = new Authenticator(RequestService);
        }

        private HttpClient HttpClient { get; }
        private RequestService RequestService { get; }
        private Authenticator Authenticator { get; }
        private string GetUserUrl { get; } = "/users";

        [Fact]
        public async Task GetUsers_CorrectData_ReturnsUsers()
        {
            Dictionary<string, UserInfo> users = new Users().Data;
            AuthenticateResponse authenticateResponse = await Authenticator.AuthenticateAsync(HttpClient);

            (HttpStatusCode httpStatusCode, List<UserDto>? userDto) = await RequestService.SendGetAsync<List<UserDto>>(
                GetUserUrl,
                HttpClient,
                authenticateResponse.AccessToken
            );

            Assert.Equal(HttpStatusCode.OK, httpStatusCode);
            Assert.Equal(users.Count + 1, userDto?.Count);
        }

        [Fact]
        public async Task GetUsers_WithoutAuthenticationToken_Unauthorized()
        {
            (HttpStatusCode httpStatusCode, List<UserDto>? userDto) = await RequestService.SendGetAsync<List<UserDto>>(
                GetUserUrl,
                HttpClient
            );

            Assert.Equal(HttpStatusCode.Unauthorized, httpStatusCode);
            Assert.Null(userDto);
        }

        [Fact]
        public async Task GetUsers_UserWithoutRoleAdmin_Forbidden()
        {
            UserInfo user = new Users().Data["test4@lukaszrydzkowski.pl"];
            AuthenticateResponse authenticateResponse = await Authenticator.AuthenticateAsync(
                HttpClient,
                new AuthenticateRequest
                {
                    Email = user.Email,
                    Password = user.Password
                }
            );

            (HttpStatusCode httpStatusCode, List<UserDto>? userDto) = await RequestService.SendGetAsync<List<UserDto>>(
                GetUserUrl,
                HttpClient,
                authenticateResponse.AccessToken
            );

            Assert.Equal(HttpStatusCode.Forbidden, httpStatusCode);
            Assert.Null(userDto);
        }
    }
}
