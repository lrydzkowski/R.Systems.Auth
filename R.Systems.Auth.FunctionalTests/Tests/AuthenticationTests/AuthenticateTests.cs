using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using R.Systems.Auth.Core.Models.Users;
using R.Systems.Auth.FunctionalTests.Initializers;
using R.Systems.Auth.FunctionalTests.Models;
using R.Systems.Auth.FunctionalTests.Services;
using R.Systems.Auth.WebApi;
using R.Systems.Auth.WebApi.Features.Authentication;
using R.Systems.Auth.WebApi.Features.Tokens;
using R.Systems.Shared.Core.Validation;
using Xunit;
using UserSettings = R.Systems.Auth.FunctionalTests.Models.UserSettings;

namespace R.Systems.Auth.FunctionalTests.Tests.AuthenticationTests;

public class AuthenticateTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    public AuthenticateTests(CustomWebApplicationFactory<Program> webApplicationFactory)
    {
        HttpClient = webApplicationFactory.CreateClient();
        RequestService = new RequestService();
        Authenticator = new Authenticator(RequestService);
        JwtTokenService = new JwtTokenService();
    }

    private string AuthenticateUrl { get; } = "/users/authenticate";
    private string GenerateNewTokensUrl { get; } = "/users/generate-new-tokens";
    private string GetUserUrl { get; } = "/users/1";
    private HttpClient HttpClient { get; }
    private RequestService RequestService { get; }
    private Authenticator Authenticator { get; }
    private JwtTokenService JwtTokenService { get; }

    [Theory]
    [InlineData("test32@lukaszrydzkowski.pl")]
    public async Task Authenticate_WrongEmail_Unauthorized(string email)
    {
        AuthenticateRequest request = new()
        {
            Email = email,
            Password = new Users().Data["test@lukaszrydzkowski.pl"].Password
        };

        (HttpStatusCode httpStatusCode, _) = await RequestService.SendPostAsync
            <AuthenticateRequest, AuthenticateResponse>(
                AuthenticateUrl,
                request,
                HttpClient
            );

        Assert.Equal(HttpStatusCode.Unauthorized, httpStatusCode);
    }

    [Theory]
    [InlineData("1231231")]
    public async Task Authenticate_WrongPassword_Unauthorized(string password)
    {
        AuthenticateRequest request = new()
        {
            Email = new Users().Data["test2@lukaszrydzkowski.pl"].Password,
            Password = password
        };

        (HttpStatusCode httpStatusCode, _) = await RequestService.SendPostAsync
            <AuthenticateRequest, AuthenticateResponse>(
                AuthenticateUrl,
                request,
                HttpClient
            );

        Assert.Equal(HttpStatusCode.Unauthorized, httpStatusCode);
    }

    [Theory]
    [MemberData(nameof(GetParametersFor_Authenticate_CorrectData))]
    public async Task Authenticate_CorrectData_GetTokens(string email, string password)
    {
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

        Assert.Equal(HttpStatusCode.OK, httpStatusCode);
        Assert.NotNull(response);
        Assert.False(string.IsNullOrEmpty(response?.AccessToken));
        Assert.False(string.IsNullOrEmpty(response?.RefreshToken));
    }

    public static IEnumerable<object[]> GetParametersFor_Authenticate_CorrectData()
    {
        Users users = new();
        return new List<object[]>
        {
            new object[]
            {
                users.Data["test@lukaszrydzkowski.pl"].Email,
                users.Data["test@lukaszrydzkowski.pl"].Password
            },
            new object[]
            {
                users.Data["test2@lukaszrydzkowski.pl"].Email,
                ""
            },
            new object[]
            {
                $" {users.Data["test2@lukaszrydzkowski.pl"].Email} ",
                $" {users.Data["test2@lukaszrydzkowski.pl"].Password} "
            }
        };
    }

    [Fact]
    public async Task Authenticate_IncorrectLoginsInARow_UserIsBlocked()
    {
        UserInfo user = new Users().Data["test8@lukaszrydzkowski.pl"];
        AuthenticateRequest incorrectRequest = new()
        {
            Email = user.Email,
            Password = user.Password + "1"
        };
        int numOfIncorrectLogins = UserSettings.MaxNumOfIncorrectLoginsBeforeBlock + 1;

        HttpStatusCode? httpStatusCodeAfterBlockade = null;
        List<ErrorInfo>? blockadeErrors = null;
        for (int i = 0; i < numOfIncorrectLogins; i++)
        {
            (httpStatusCodeAfterBlockade, blockadeErrors) = await RequestService.SendPostAsync
                <AuthenticateRequest, List<ErrorInfo>>(
                    AuthenticateUrl,
                    incorrectRequest,
                    HttpClient
                );
        }

        Assert.Equal(HttpStatusCode.BadRequest, httpStatusCodeAfterBlockade);
        Assert.NotNull(blockadeErrors);
        blockadeErrors.Should().BeEquivalentTo(
            new List<ErrorInfo>
            {
                new(errorKey: "Blocked", elementKey: "User")
            }
        );
    }

    [Fact]
    public async Task GenerateNewTokens_CorrectRefreshToken_GetNewTokens()
    {
        AuthenticateResponse authenticateResponse = await Authenticator.AuthenticateAsync(HttpClient);
        GenerateNewTokensRequest newTokensRequest = new()
        {
            RefreshToken = authenticateResponse.RefreshToken
        };

        (HttpStatusCode httpStatusCode, GenerateNewTokensResponse? response) = await RequestService.SendPostAsync
            <GenerateNewTokensRequest, GenerateNewTokensResponse>(
                GenerateNewTokensUrl,
                newTokensRequest,
                HttpClient
            );
        (HttpStatusCode getHttpUserStatusCode, _) = await RequestService.SendGetAsync<UserDto>(
            GetUserUrl,
            HttpClient,
            response?.AccessToken
        );

        Assert.Equal(HttpStatusCode.OK, httpStatusCode);
        Assert.NotNull(response);
        Assert.False(string.IsNullOrEmpty(response?.AccessToken));
        Assert.False(string.IsNullOrEmpty(response?.RefreshToken));
        Assert.Equal(HttpStatusCode.OK, getHttpUserStatusCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData("13rfewghrgr")]
    public async Task GenerateNewTokens_IncorrectRefreshToken_Unauthorized(string refreshToken)
    {
        GenerateNewTokensRequest newTokensRequest = new()
        {
            RefreshToken = refreshToken
        };

        (HttpStatusCode httpStatusCode, _) = await RequestService.SendPostAsync
            <GenerateNewTokensRequest, GenerateNewTokensResponse>(
                GenerateNewTokensUrl,
                newTokensRequest,
                HttpClient
            );

        Assert.Equal(HttpStatusCode.Unauthorized, httpStatusCode);
    }

    [Fact]
    public async Task GenerateNewTokens_ExpiredRefreshToken_Unauthorized()
    {
        GenerateNewTokensRequest newTokensRequest = new()
        {
            RefreshToken = new Users().Data["test3@lukaszrydzkowski.pl"].RefreshToken ?? ""
        };

        (HttpStatusCode httpStatusCode, _) = await RequestService.SendPostAsync
            <GenerateNewTokensRequest, GenerateNewTokensResponse>(
                GenerateNewTokensUrl,
                newTokensRequest,
                HttpClient
            );

        Assert.Equal(HttpStatusCode.Unauthorized, httpStatusCode);
    }

    [Fact]
    public async Task UseAccessToken_ExpiredAccessToken_Unauthorized()
    {
        AuthenticateResponse authenticateResponse = await Authenticator.AuthenticateAsync(HttpClient);

        (HttpStatusCode httpStatusCode, _) = await RequestService.SendGetAsync<UserDto>(
            GetUserUrl,
            HttpClient,
            JwtTokenService.TamperAccessTokenExpireDate(authenticateResponse.AccessToken, -15)
        );

        Assert.Equal(HttpStatusCode.Unauthorized, httpStatusCode);
    }
}
