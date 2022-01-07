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

namespace R.Systems.Auth.FunctionalTests.Tests.RoleTests;

public class GetRolesTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    public GetRolesTests(CustomWebApplicationFactory<Program> webApplicationFactory)
    {
        HttpClient = webApplicationFactory.CreateClient();
        RequestService = new RequestService();
        Authenticator = new Authenticator(RequestService);
    }

    private HttpClient HttpClient { get; }
    private RequestService RequestService { get; }
    private Authenticator Authenticator { get; }
    private string GetUserUrl { get; } = "/roles";

    [Fact]
    public async Task GetRoles_CorrectData_ReturnsRoles()
    {
        AuthenticateResponse authenticateResponse = await Authenticator.AuthenticateAsync(HttpClient);

        (HttpStatusCode httpStatusCode, List<RoleDto>? roleDto) = await RequestService.SendGetAsync<List<RoleDto>>(
            GetUserUrl,
            HttpClient,
            authenticateResponse.AccessToken
        );

        Assert.Equal(HttpStatusCode.OK, httpStatusCode);
        Assert.Equal(2, roleDto?.Count);
    }

    [Fact]
    public async Task GetRoles_WithoutAuthenticationToken_Unauthorized()
    {
        (HttpStatusCode httpStatusCode, List<RoleDto>? roleDto) = await RequestService.SendGetAsync<List<RoleDto>>(
            GetUserUrl,
            HttpClient
        );

        Assert.Equal(HttpStatusCode.Unauthorized, httpStatusCode);
        Assert.Null(roleDto);
    }

    [Fact]
    public async Task GetRoles_UserWithoutRoleAdmin_Forbidden()
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

        (HttpStatusCode httpStatusCode, List<RoleDto>? roleDto) = await RequestService.SendGetAsync<List<RoleDto>>(
            GetUserUrl,
            HttpClient,
            authenticateResponse.AccessToken
        );

        Assert.Equal(HttpStatusCode.Forbidden, httpStatusCode);
        Assert.Null(roleDto);
    }
}
