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
using R.Systems.Shared.Core.Validation;
using Xunit;

namespace R.Systems.Auth.FunctionalTests.Tests.UserTests;

public class ChangeUserPasswordTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    public ChangeUserPasswordTests(CustomWebApplicationFactory<Program> webApplicationFactory)
    {
        HttpClient = webApplicationFactory.CreateClient();
        RequestService = new RequestService();
        Authenticator = new Authenticator(RequestService);
    }

    public HttpClient HttpClient { get; }
    public RequestService RequestService { get; }
    public Authenticator Authenticator { get; }
    private string ChangePasswordUrl { get; } = "/users/change-password";

    [Fact]
    public async Task ChangeUserPassword_WithoutAuthenticationToken_Unauthorized()
    {
        ChangeUserPasswordDto changeUserPasswordDto = new();
        (HttpStatusCode httpStatusCode, _) = await RequestService.SendPostAsync<ChangeUserPasswordDto, object>(
            ChangePasswordUrl,
            changeUserPasswordDto,
            HttpClient
        );

        Assert.Equal(HttpStatusCode.Unauthorized, httpStatusCode);
    }

    [Fact]
    public async Task ChangeUserPassword_CorrectData_ReturnsOk()
    {
        UserInfo user = new Users().Data["test6@lukaszrydzkowski.pl"];
        string newPassword = "d22d22";
        AuthenticateRequest authRequest = new()
        {
            Email = user.Email,
            Password = user.Password
        };
        AuthenticateResponse authResponse = await Authenticator.AuthenticateAsync(
            HttpClient, authRequest
        );
        ChangeUserPasswordDto changeUserPasswordDto = new()
        {
            CurrentPassword = user.Password,
            NewPassword = newPassword,
            RepeatedNewPassword = newPassword
        };

        (HttpStatusCode changePasswordHttpStatusCode, _)
            = await RequestService.SendPostAsync<ChangeUserPasswordDto, object>(
                ChangePasswordUrl,
                changeUserPasswordDto,
                HttpClient,
                authResponse.AccessToken
            );
        AuthenticateRequest authRequestAfter = new()
        {
            Email = user.Email,
            Password = newPassword
        };
        AuthenticateResponse authResponseAfter = await Authenticator.AuthenticateAsync(
            HttpClient, authRequestAfter
        );

        Assert.Equal(HttpStatusCode.OK, changePasswordHttpStatusCode);
        Assert.False(string.IsNullOrEmpty(authResponseAfter.AccessToken));
        Assert.False(string.IsNullOrEmpty(authResponseAfter.RefreshToken));
    }

    [Theory]
    [MemberData(nameof(GetParametersFor_ChangeUserPassword_IncorrectData))]
    public async Task ChangeUserPassword_IncorrectData_ReturnsErrorsList(
        string newPassword, string repeatedNewPassword, List<ErrorInfo> expectedErrors, string? oldPassword = null)
    {
        UserInfo user = new Users().Data["test7@lukaszrydzkowski.pl"];
        AuthenticateRequest authRequest = new()
        {
            Email = user.Email,
            Password = user.Password
        };
        AuthenticateResponse authResponse = await Authenticator.AuthenticateAsync(
            HttpClient, authRequest
        );
        ChangeUserPasswordDto changeUserPasswordDto = new()
        {
            CurrentPassword = oldPassword ?? user.Password,
            NewPassword = newPassword,
            RepeatedNewPassword = repeatedNewPassword
        };

        (HttpStatusCode changePasswordHttpStatusCode, List<ErrorInfo>? errors)
            = await RequestService.SendPostAsync<ChangeUserPasswordDto, List<ErrorInfo>>(
                ChangePasswordUrl,
                changeUserPasswordDto,
                HttpClient,
                authResponse.AccessToken
            );

        Assert.Equal(HttpStatusCode.BadRequest, changePasswordHttpStatusCode);
        errors.Should().BeEquivalentTo(expectedErrors);
    }

    private static IEnumerable<object?[]> GetParametersFor_ChangeUserPassword_IncorrectData()
    {
        return new List<object?[]>
        {
            new object?[]
            {
                "d11d11",
                "d11d11",
                new List<ErrorInfo> { new(errorKey: "WrongPassword", elementKey: "User") },
                "ddd"
            },
            new object?[]
            {
                "d11d11",
                "d11d112",
                new List<ErrorInfo> { new(errorKey: "DifferentValues", elementKey: "Passwords") }
            },
            new object?[]
            {
                "",
                "",
                new List<ErrorInfo> { new(errorKey: "IsRequired", elementKey: "Password") }
            },
            new object?[]
            {
                "d11",
                "d11",
                new List<ErrorInfo> { new(errorKey: "TooShort", elementKey: "Password") }
            },
            new object?[]
            {
                "3030303030303030303030303030303",
                "3030303030303030303030303030303",
                new List<ErrorInfo> { new(errorKey: "TooLong", elementKey: "Password") }
            }
        };
    }
}
