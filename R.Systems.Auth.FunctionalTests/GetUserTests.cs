using FluentAssertions;
using R.Systems.Auth.Core.Models;
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
    public class GetUserTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        public GetUserTests(CustomWebApplicationFactory<Startup> webApplicationFactory)
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
        public async Task GetUser_PassCorrectId_ReturnsUserData()
        {
            Roles roles = new();
            UserInfo user = new Users().Data["test@lukaszrydzkowski.pl"];
            string roleKey = user.RoleKeys[0];
            AuthenticateResponse authenticateResponse = await Authenticator.AuthenticateAsync(HttpClient);

            (HttpStatusCode httpStatusCode, UserDto? userDto) = await RequestService.SendGetAsync<UserDto>(
                GetUserUrl + $"/{user.UserId}",
                HttpClient,
                authenticateResponse.AccessToken
            );

            Assert.Equal(HttpStatusCode.OK, httpStatusCode);
            Assert.NotNull(userDto);
            userDto.Should()
                .BeEquivalentTo(new UserDto
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = new List<RoleDto>()
                    {
                        new RoleDto()
                        {
                            RoleId = roles.Data[roleKey].RoleId,
                            RoleKey = roles.Data[roleKey].RoleKey,
                            Name = roles.Data[roleKey].Name,
                            Description = roles.Data[roleKey].Description
                        }
                    }
                });
        }

        [Fact]
        public async Task GetUser_PassNotExistingId_NotExists()
        {
            long userId = 999;
            AuthenticateResponse authenticateResponse = await Authenticator.AuthenticateAsync(HttpClient);

            (HttpStatusCode httpStatusCode, UserDto? userDto) = await RequestService.SendGetAsync<UserDto>(
                GetUserUrl + $"/{userId}",
                HttpClient,
                authenticateResponse.AccessToken
            );

            Assert.Equal(HttpStatusCode.NotFound, httpStatusCode);
            Assert.Null(userDto);
        }

        [Fact]
        public async Task GetUser_WithoutAuthenticationToken_Unauthorized()
        {
            UserInfo user = new Users().Data["test@lukaszrydzkowski.pl"];
            (HttpStatusCode httpStatusCode, UserDto? userDto) = await RequestService.SendGetAsync<UserDto>(
                GetUserUrl + $"/{user.UserId}",
                HttpClient
            );

            Assert.Equal(HttpStatusCode.Unauthorized, httpStatusCode);
            Assert.Null(userDto);
        }

        [Fact]
        public async Task GetUser_UserWithoutRoleAdmin_Forbidden()
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

            (HttpStatusCode httpStatusCode, UserDto? userDto) = await RequestService.SendGetAsync<UserDto>(
                GetUserUrl + $"/1",
                HttpClient,
                authenticateResponse.AccessToken
            );

            Assert.Equal(HttpStatusCode.Forbidden, httpStatusCode);
            Assert.Null(userDto);
        }
    }
}
