﻿using FluentAssertions;
using R.Systems.Auth.FunctionalTests.Initializers;
using R.Systems.Auth.FunctionalTests.Models;
using R.Systems.Auth.FunctionalTests.Services;
using R.Systems.Auth.WebApi;
using R.Systems.Auth.WebApi.Features.Authentication;
using R.Systems.Auth.WebApi.Features.User;
using R.Systems.Shared.Core.Validation;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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
        ChangePasswordRequest request = new();
        (HttpStatusCode httpStatusCode, _) = await RequestService.SendPostAsync<ChangePasswordRequest, object>(
            ChangePasswordUrl,
            request,
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
        ChangePasswordRequest changePasswordRequest = new()
        {
            OldPassword = user.Password,
            NewPassword = newPassword,
            RepeatedNewPassword = newPassword
        };

        (HttpStatusCode changePasswordHttpStatusCode, _)
            = await RequestService.SendPostAsync<ChangePasswordRequest, object>(
                ChangePasswordUrl,
                changePasswordRequest,
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
        ChangePasswordRequest changePasswordRequest = new()
        {
            OldPassword = oldPassword ?? user.Password,
            NewPassword = newPassword,
            RepeatedNewPassword = repeatedNewPassword
        };

        (HttpStatusCode changePasswordHttpStatusCode, List<ErrorInfo>? errors)
            = await RequestService.SendPostAsync<ChangePasswordRequest, List<ErrorInfo>>(
                ChangePasswordUrl,
                changePasswordRequest,
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
                new List<ErrorInfo>() { new ErrorInfo(errorKey: "WrongPassword", elementKey: "User") },
                "ddd"
            },
            new object?[]
            {
                "d11d11",
                "d11d112",
                new List<ErrorInfo>() { new ErrorInfo(errorKey: "DifferentValues", elementKey: "Passwords") }
            },
            new object?[]
            {
                "",
                "",
                new List<ErrorInfo>() { new ErrorInfo(errorKey: "IsRequired", elementKey: "Password") }
            },
            new object?[]
            {
                "d11",
                "d11",
                new List<ErrorInfo>() { new ErrorInfo(errorKey: "TooShort", elementKey: "Password") }
            },
            new object?[]
            {
                "3030303030303030303030303030303",
                "3030303030303030303030303030303",
                new List<ErrorInfo>() { new ErrorInfo(errorKey: "TooLong", elementKey: "Password") }
            }
        };
    }
}
