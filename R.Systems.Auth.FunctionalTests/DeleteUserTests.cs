using FluentAssertions;
using R.Systems.Auth.Core.Models;
using R.Systems.Auth.FunctionalTests.Initializers;
using R.Systems.Auth.FunctionalTests.Models;
using R.Systems.Auth.FunctionalTests.Services;
using R.Systems.Auth.SharedKernel.Validation;
using R.Systems.Auth.WebApi.Features.Authentication;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace R.Systems.Auth.FunctionalTests
{
    public class DeleteUserTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        public DeleteUserTests(CustomWebApplicationFactory<Startup> webApplicationFactory)
        {
            HttpClient = webApplicationFactory.CreateClient();
            RequestService = new RequestService();
            Authenticator = new Authenticator(RequestService);
        }

        public HttpClient HttpClient { get; }
        public RequestService RequestService { get; }
        public Authenticator Authenticator { get; }
        public string UsersUrl { get; } = "/users";

        [Fact]
        public async Task DeleteUser_WithoutAuthenticationToken_Unauthorized()
        {
            User userToDelete = new Users().Data["test5@lukaszrydzkowski.pl"];

            (HttpStatusCode httpStatusCode, _) = await RequestService.SendDeleteAsync<object>(
                $"{UsersUrl}/{userToDelete.Id}",
                HttpClient
            );

            Assert.Equal(HttpStatusCode.Unauthorized, httpStatusCode);
        }

        [Fact]
        public async Task DeleteUser_UserWithoutRoleAdmin_Forbidden()
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
            User userToDelete = new Users().Data["test@lukaszrydzkowski.pl"];

            (HttpStatusCode httpStatusCode, _) = await RequestService.SendDeleteAsync<object>(
                $"{UsersUrl}/{userToDelete.Id}",
                HttpClient,
                authenticateResponse.AccessToken
            );

            Assert.Equal(HttpStatusCode.Forbidden, httpStatusCode);
        }

        [Fact]
        public async Task DeleteUser_CorrectData_ReturnsOk()
        {
            UserInfo user = new Users().Data["test@lukaszrydzkowski.pl"];
            AuthenticateResponse authenticateResponse = await Authenticator.AuthenticateAsync(
                HttpClient,
                new AuthenticateRequest
                {
                    Email = user.Email,
                    Password = user.Password
                }
            );
            UserInfo userToDelete = new Users().Data["test5@lukaszrydzkowski.pl"];

            (HttpStatusCode getUserBeforeHttpStatusCode, _) = await RequestService.SendGetAsync<UserDto>(
                $"{UsersUrl}/{userToDelete.Id}",
                HttpClient,
                authenticateResponse.AccessToken
            );
            (HttpStatusCode deleteHttpStatusCode, _) = await RequestService.SendDeleteAsync<object>(
                $"{UsersUrl}/{userToDelete.Id}",
                HttpClient,
                authenticateResponse.AccessToken
            );
            (HttpStatusCode getUserAfterHttpStatusCode, _) = await RequestService.SendGetAsync<UserDto>(
                $"{UsersUrl}/{userToDelete.Id}",
                HttpClient,
                authenticateResponse.AccessToken
            );

            Assert.Equal(HttpStatusCode.OK, getUserBeforeHttpStatusCode);
            Assert.Equal(HttpStatusCode.OK, deleteHttpStatusCode);
            Assert.Equal(HttpStatusCode.NotFound, getUserAfterHttpStatusCode);
        }

        [Theory]
        [MemberData(nameof(DeleteUserIncorrectDataParameters))]
        public async Task DeleteUser_IncorrectData_ReturnsErrorsList(
            UserInfo userToLogin, long userIdToDelete, List<ErrorInfo> expectedErrors)
        {
            AuthenticateResponse authenticateResponse = await Authenticator.AuthenticateAsync(
                HttpClient,
                new AuthenticateRequest
                {
                    Email = userToLogin.Email,
                    Password = userToLogin.Password
                }
            );

            (HttpStatusCode deleteHttpStatusCode, List<ErrorInfo>? deleteUserErrorResponse) =
                await RequestService.SendDeleteAsync<List<ErrorInfo>>(
                    $"{UsersUrl}/{userIdToDelete}",
                    HttpClient,
                    authenticateResponse.AccessToken
                );

            Assert.Equal(HttpStatusCode.BadRequest, deleteHttpStatusCode);
            deleteUserErrorResponse.Should().BeEquivalentTo(expectedErrors);
        }

        public static IEnumerable<object[]> DeleteUserIncorrectDataParameters()
        {
            UserInfo user = new Users().Data["test@lukaszrydzkowski.pl"];
            return new List<object[]>
            {
                new object[]
                {
                    user,
                    999,
                    new List<ErrorInfo>()
                    {
                        new ErrorInfo(errorKey: "NotExist", elementKey: "UserId")
                    }
                },
                new object[]
                {
                    user,
                    user.Id,
                    new List<ErrorInfo>()
                    {
                        new ErrorInfo(errorKey: "CannotDeleteYourself", elementKey: "UserId")
                    }
                }
            };
        }
    }
}
