using FluentAssertions;
using R.Systems.Auth.Core.Models.Roles;
using R.Systems.Auth.Core.Models.Users;
using R.Systems.Auth.FunctionalTests.Initializers;
using R.Systems.Auth.FunctionalTests.Models;
using R.Systems.Auth.FunctionalTests.Services;
using R.Systems.Auth.WebApi;
using R.Systems.Auth.WebApi.Features.Authentication;
using R.Systems.Shared.Core.Validation;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace R.Systems.Auth.FunctionalTests.Tests.UserTests;

public class UpdateUserTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    public UpdateUserTests(CustomWebApplicationFactory<Program> webApplicationFactory)
    {
        HttpClient = webApplicationFactory.CreateClient();
        RequestService = new RequestService();
        Authenticator = new Authenticator(RequestService);
    }

    public HttpClient HttpClient { get; }
    public RequestService RequestService { get; }
    public Authenticator Authenticator { get; }
    public string UsersUrl { get; } = "/users";
    private string AuthenticateUrl { get; } = "/users/authenticate";

    [Fact]
    public async Task UpdateUser_WithoutAuthenticationToken_Unauthorized()
    {
        EditUserDto editUserDto = new();
        UserInfo user = new Users().Data["test@lukaszrydzkowski.pl"];

        (HttpStatusCode httpStatusCode, _) = await RequestService.SendPostAsync<EditUserDto, object>(
            $"{UsersUrl}/${user.Id}",
            editUserDto,
            HttpClient
        );

        Assert.Equal(HttpStatusCode.Unauthorized, httpStatusCode);
    }

    [Fact]
    public async Task UpdateUser_UserWithoutRoleAdmin_Forbidden()
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
        EditUserDto editUserDto = new();

        (HttpStatusCode httpStatusCode, _) = await RequestService.SendPostAsync<EditUserDto, object>(
            $"{UsersUrl}/${user.Id}",
            editUserDto,
            HttpClient,
            authenticateResponse.AccessToken
        );

        Assert.Equal(HttpStatusCode.Forbidden, httpStatusCode);
    }

    [Theory]
    [MemberData(nameof(GetParametersFor_UpdateUser_CorrectData))]
    public async Task UpdateUser_CorrectData_ReturnsOk(EditUserDto editUserDto, UserDto expectedUserDto)
    {
        UserInfo loggedUser = new Users().Data["test@lukaszrydzkowski.pl"];
        AuthenticateResponse authenticateResponse = await Authenticator.AuthenticateAsync(
            HttpClient,
            new AuthenticateRequest
            {
                Email = loggedUser.Email,
                Password = loggedUser.Password
            }
        );
        List<RoleDto> roles = new Roles().Data
            .Select(x => new RoleDto()
            {
                RoleId = x.Value.Id,
                RoleKey = x.Value.RoleKey,
                Name = x.Value.Name,
                Description = x.Value.Description
            })
            .ToList();
        UserInfo userToEdit = new Users().Data["test2@lukaszrydzkowski.pl"];

        (HttpStatusCode updateUserHttpStatusCode, _) = await RequestService.SendPostAsync<EditUserDto, object>(
            $"{UsersUrl}/{userToEdit.Id}",
            editUserDto,
            HttpClient,
            authenticateResponse.AccessToken
        );
        (HttpStatusCode getUserHttpStatusCode, UserDto? userDto) = await RequestService.SendGetAsync<UserDto>(
            $"{UsersUrl}/{userToEdit.Id}",
            HttpClient,
            authenticateResponse.AccessToken
        );

        Assert.Equal(HttpStatusCode.OK, updateUserHttpStatusCode);
        Assert.Equal(HttpStatusCode.OK, getUserHttpStatusCode);
        Assert.NotNull(userDto);
        userDto.Should().BeEquivalentTo(expectedUserDto);
    }

    public static IEnumerable<object[]> GetParametersFor_UpdateUser_CorrectData()
    {
        UserInfo user = new Users().Data["test2@lukaszrydzkowski.pl"];

        RoleEntity adminRole = new Roles().Data["admin"];
        RoleDto adminRoleDto = new()
        {
            RoleId = adminRole.Id,
            RoleKey = adminRole.RoleKey,
            Name = adminRole.Name,
            Description = adminRole.Description
        };
        RoleEntity userRole = new Roles().Data["user"];
        RoleDto userRoleDto = new()
        {
            RoleId = userRole.Id,
            RoleKey = userRole.RoleKey,
            Name = userRole.Name,
            Description = userRole.Description
        };

        return new List<object[]>
        {
            new object[]
            {
                new EditUserDto
                {
                    RoleIds = new List<long>() { userRole.Id }
                },
                new UserDto
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = new List<RoleDto>() { userRoleDto }
                }
            },
            new object[]
            {
                new EditUserDto
                {
                    Email = "qwerty123@gmail.com"
                },
                new UserDto
                {
                    UserId = user.Id,
                    Email = "qwerty123@gmail.com",
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = new List<RoleDto>() { userRoleDto }
                }
            },
            new object[]
            {
                new EditUserDto
                {
                    FirstName = "Kacper"
                },
                new UserDto
                {
                    UserId = user.Id,
                    Email = "qwerty123@gmail.com",
                    FirstName = "Kacper",
                    LastName = user.LastName,
                    Roles = new List<RoleDto>() { userRoleDto }
                }
            },
            new object[]
            {
                new EditUserDto
                {
                    LastName = "Grudziński"
                },
                new UserDto
                {
                    UserId = user.Id,
                    Email = "qwerty123@gmail.com",
                    FirstName = "Kacper",
                    LastName = "Grudziński",
                    Roles = new List<RoleDto>() { userRoleDto }
                }
            },
            new object[]
            {
                new EditUserDto
                {
                    RoleIds = new List<long>() { adminRole.Id }
                },
                new UserDto
                {
                    UserId = user.Id,
                    Email = "qwerty123@gmail.com",
                    FirstName = "Kacper",
                    LastName = "Grudziński",
                    Roles = new List<RoleDto>() { adminRoleDto }
                }
            },
            new object[]
            {
                new EditUserDto
                {
                    RoleIds = new List<long>() { adminRole.Id, userRole.Id }
                },
                new UserDto
                {
                    UserId = user.Id,
                    Email = "qwerty123@gmail.com",
                    FirstName = "Kacper",
                    LastName = "Grudziński",
                    Roles = new List<RoleDto>() { adminRoleDto, userRoleDto }
                }
            },
            new object[]
            {
                new EditUserDto
                {
                    RoleIds = new List<long>() { userRole.Id }
                },
                new UserDto
                {
                    UserId = user.Id,
                    Email = "qwerty123@gmail.com",
                    FirstName = "Kacper",
                    LastName = "Grudziński",
                    Roles = new List<RoleDto>() { userRoleDto }
                }
            }
        };
    }

    [Theory]
    [MemberData(nameof(GetParametersFor_UpdateUser_IncorrectData))]
    public async Task UpdateUser_IncorrectData_ReturnsErrorsList(
        EditUserDto editUserDto,
        long userId,
        List<ErrorInfo> expectedErrors)
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

        (HttpStatusCode updateUserHttpStatusCode, List<ErrorInfo>? updateUserErrorResponse) =
            await RequestService.SendPostAsync<EditUserDto, List<ErrorInfo>>(
                $"{UsersUrl}/{userId}",
                editUserDto,
                HttpClient,
                authenticateResponse.AccessToken
            );

        Assert.Equal(HttpStatusCode.BadRequest, updateUserHttpStatusCode);
        updateUserErrorResponse.Should().BeEquivalentTo(expectedErrors);
    }

    public static IEnumerable<object[]> GetParametersFor_UpdateUser_IncorrectData()
    {
        UserInfo user = new Users().Data["test2@lukaszrydzkowski.pl"];
        UserInfo user2 = new Users().Data["test3@lukaszrydzkowski.pl"];
        return new List<object[]>
        {
            new object[]
            {
                new EditUserDto()
                {
                    Email = "123@gmail.com"
                },
                999,
                new List<ErrorInfo>()
                {
                    new ErrorInfo(errorKey: "NotExist", elementKey: "UserId")
                }
            },
            new object[]
            {
                new EditUserDto()
                {
                    Email = " "
                },
                user.Id,
                new List<ErrorInfo>()
                {
                    new ErrorInfo(errorKey: "IsRequired", elementKey: "Email")
                }
            },
            new object[]
            {
                new EditUserDto()
                {
                    Email = "test"
                },
                user.Id,
                new List<ErrorInfo>()
                {
                    new ErrorInfo(errorKey: "WrongStructure", elementKey: "Email")
                }
            },
            new object[]
            {
                new EditUserDto()
                {
                    Email = "200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200@gmail.com"
                },
                user.Id,
                new List<ErrorInfo>()
                {
                    new ErrorInfo(errorKey: "TooLong", elementKey: "Email")
                }
            },
            new object[]
            {
                new EditUserDto()
                {
                    Email = user2.Email
                },
                user.Id,
                new List<ErrorInfo>()
                {
                    new ErrorInfo(errorKey: "Exists", elementKey: "Email")
                }
            },
            new object[]
            {
                new EditUserDto()
                {
                    FirstName = ""
                },
                user.Id,
                new List<ErrorInfo>()
                {
                    new ErrorInfo(errorKey: "IsRequired", elementKey: "FirstName")
                }
            },
            new object[]
            {
                new EditUserDto()
                {
                    FirstName = "200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200"
                },
                user.Id,
                new List<ErrorInfo>()
                {
                    new ErrorInfo(errorKey: "TooLong", elementKey: "FirstName")
                }
            },
            new object[]
            {
                new EditUserDto()
                {
                    LastName = ""
                },
                user.Id,
                new List<ErrorInfo>()
                {
                    new ErrorInfo(errorKey: "IsRequired", elementKey: "LastName")
                }
            },
            new object[]
            {
                new EditUserDto()
                {
                    LastName = "200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200"
                },
                user.Id,
                new List<ErrorInfo>()
                {
                    new ErrorInfo(errorKey: "TooLong", elementKey: "LastName")
                }
            },
            new object[]
            {
                new EditUserDto()
                {
                    Password = ""
                },
                user.Id,
                new List<ErrorInfo>()
                {
                    new ErrorInfo(errorKey: "IsRequired", elementKey: "Password")
                }
            },
            new object[]
            {
                new EditUserDto()
                {
                    Password = "1234"
                },
                user.Id,
                new List<ErrorInfo>()
                {
                    new ErrorInfo(errorKey: "TooShort", elementKey: "Password")
                }
            },
            new object[]
            {
                new EditUserDto()
                {
                    Password = "1234567890123456789012345678901"
                },
                user.Id,
                new List<ErrorInfo>()
                {
                    new ErrorInfo(errorKey: "TooLong", elementKey: "Password")
                }
            },
            new object[]
            {
                new EditUserDto()
                {
                    RoleIds = new List<long>()
                },
                user.Id,
                new List<ErrorInfo>()
                {
                    new ErrorInfo(errorKey: "IsRequired", elementKey: "RoleId")
                }
            },
            new object[]
            {
                new EditUserDto()
                {
                    RoleIds = new List<long>() { 50 }
                },
                user.Id,
                new List<ErrorInfo>()
                {
                    new ErrorInfo(
                        errorKey: "NotExist",
                        elementKey: "RoleId",
                        data: new Dictionary<string, string>()
                        {
                            { "RoleId", "50" }
                        }
                    )
                }
            },
            new object[]
            {
                new EditUserDto()
                {
                    Email = "",
                    FirstName = "Lucas",
                    LastName = "WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW",
                    Password = "12345678901234567890123456789012",
                    RoleIds = new List<long>() { 120 }
                },
                user.Id,
                new List<ErrorInfo>()
                {
                    new ErrorInfo(errorKey: "IsRequired", elementKey: "Email"),
                    new ErrorInfo(errorKey: "TooLong", elementKey: "LastName"),
                    new ErrorInfo(errorKey: "TooLong", elementKey: "Password"),
                    new ErrorInfo(
                        errorKey: "NotExist",
                        elementKey: "RoleId",
                        data: new Dictionary<string, string>()
                        {
                            { "RoleId", "120" }
                        }
                    )
                }
            }
        };
    }
}
