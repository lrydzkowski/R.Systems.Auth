using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using R.Systems.Auth.Core.Models.Roles;
using R.Systems.Auth.Core.Models.Users;
using R.Systems.Auth.FunctionalTests.Initializers;
using R.Systems.Auth.FunctionalTests.Models;
using R.Systems.Auth.FunctionalTests.Services;
using R.Systems.Auth.WebApi;
using R.Systems.Auth.WebApi.Features.Authentication;
using Xunit;

namespace R.Systems.Auth.FunctionalTests.Tests.UserTests;

public class GetUserTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    public GetUserTests(CustomWebApplicationFactory<Program> webApplicationFactory)
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
    public async Task GetUser_CorrectId_ReturnsUserData()
    {
        Roles roles = new();
        UserInfo user = new Users().Data["test@lukaszrydzkowski.pl"];
        string roleKey = user.RoleKeys[0];
        AuthenticateResponse authenticateResponse = await Authenticator.AuthenticateAsync(HttpClient);

        (HttpStatusCode httpStatusCode, UserDto? userDto) = await RequestService.SendGetAsync<UserDto>(
            GetUserUrl + $"/{user.Id}",
            HttpClient,
            authenticateResponse.AccessToken
        );

        Assert.Equal(HttpStatusCode.OK, httpStatusCode);
        Assert.NotNull(userDto);
        userDto.Should()
            .BeEquivalentTo(new UserDto
            {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = new List<RoleDto>
                {
                        new()
                        {
                            RoleId = roles.Data[roleKey].Id,
                            RoleKey = roles.Data[roleKey].RoleKey,
                            Name = roles.Data[roleKey].Name,
                            Description = roles.Data[roleKey].Description
                        }
                }
            });
    }

    [Fact]
    public async Task GetUser_NotExistingId_NotExists()
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
            GetUserUrl + $"/{user.Id}",
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
            GetUserUrl + "/1",
            HttpClient,
            authenticateResponse.AccessToken
        );

        Assert.Equal(HttpStatusCode.Forbidden, httpStatusCode);
        Assert.Null(userDto);
    }
}
