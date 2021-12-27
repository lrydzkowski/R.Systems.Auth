using FluentAssertions;
using R.Systems.Auth.Core.Models;
using R.Systems.Auth.FunctionalTests.Initializers;
using R.Systems.Auth.FunctionalTests.Models;
using R.Systems.Auth.FunctionalTests.Services;
using R.Systems.Auth.WebApi;
using R.Systems.Auth.WebApi.Features.Authentication;
using R.Systems.Auth.WebApi.Features.User;
using R.Systems.Shared.Core.Validation;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace R.Systems.Auth.FunctionalTests
{
    public class CreateUserTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        public CreateUserTests(CustomWebApplicationFactory<Program> webApplicationFactory)
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
        public async Task CreateUser_WithoutAuthenticationToken_Unauthorized()
        {
            EditUserDto editUserDto = new();

            (HttpStatusCode httpStatusCode, _) = await RequestService.SendPostAsync<EditUserDto, object>(
                UsersUrl,
                editUserDto,
                HttpClient
            );

            Assert.Equal(HttpStatusCode.Unauthorized, httpStatusCode);
        }

        [Fact]
        public async Task CreateUser_UserWithoutRoleAdmin_Forbidden()
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
                UsersUrl,
                editUserDto,
                HttpClient,
                authenticateResponse.AccessToken
            );

            Assert.Equal(HttpStatusCode.Forbidden, httpStatusCode);
        }

        [Theory]
        [MemberData(nameof(CreateUserCorrectDataParameters))]
        public async Task CreateUser_CorrectData_ReturnsOk(EditUserDto editUserDto)
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
            List<RoleDto> roles = new Roles().Data
                .Select(x => new RoleDto()
                {
                    RoleId = x.Value.Id,
                    RoleKey = x.Value.RoleKey,
                    Name = x.Value.Name,
                    Description = x.Value.Description
                })
                .ToList();

            (HttpStatusCode createUserHttpStatusCode, CreateUserResponse? createUserResponse) =
                await RequestService.SendPostAsync<EditUserDto, CreateUserResponse>(
                    UsersUrl,
                    editUserDto,
                    HttpClient,
                    authenticateResponse.AccessToken
                );
            AuthenticateResponse newUserAuthenticateResponse = await Authenticator.AuthenticateAsync(
                HttpClient,
                new AuthenticateRequest
                {
                    Email = editUserDto.Email ?? "",
                    Password = editUserDto.Password ?? ""
                }
            );
            (HttpStatusCode getUserHttpStatusCode, UserDto? userDto) = await RequestService.SendGetAsync<UserDto>(
                $"{UsersUrl}/{createUserResponse?.UserId}",
                HttpClient,
                authenticateResponse.AccessToken
            );

            Assert.Equal(HttpStatusCode.OK, createUserHttpStatusCode);
            Assert.NotNull(createUserResponse);
            Assert.False(string.IsNullOrEmpty(newUserAuthenticateResponse?.AccessToken));
            Assert.False(string.IsNullOrEmpty(newUserAuthenticateResponse?.RefreshToken));
            Assert.Equal(HttpStatusCode.OK, getUserHttpStatusCode);
            Assert.NotNull(userDto);
            userDto.Should()
                .BeEquivalentTo(new UserDto
                {
                    UserId = createUserResponse?.UserId ?? 0,
                    Email = editUserDto.Email ?? "",
                    FirstName = editUserDto.FirstName ?? "",
                    LastName = editUserDto.LastName ?? "",
                    Roles = editUserDto.RoleIds == null
                        ? new List<RoleDto>()
                        : roles.Where(x => editUserDto.RoleIds.Contains(x.RoleId)).ToList()
                });
        }

        public static IEnumerable<object[]> CreateUserCorrectDataParameters()
        {
            Role adminRole = new Roles().Data["admin"];
            Role userRole = new Roles().Data["user"];
            return new List<object[]>
            {
                new object[]
                {
                    new EditUserDto()
                    {
                        Email = "test@gmail.com",
                        FirstName = "Testowy",
                        LastName = "Tester",
                        Password = "d11d11d11",
                        RoleIds = new List<long>() { adminRole.Id }
                    }
                },
                new object[]
                {
                    new EditUserDto()
                    {
                        Email = "test999999999999999999999999999999999999999999999999999@gmail.com",
                        FirstName = "T",
                        LastName = "T",
                        Password = "300300300300300300300300300300",
                        RoleIds = new List<long>() { adminRole.Id, userRole.Id }
                    }
                },
                new object[]
                {
                    new EditUserDto()
                    {
                        Email = "1234567@lukaszrydzkowski.pl",
                        FirstName = "20000200002000020000200002000020000200002000020000200002000020000",
                        LastName = "20000200002000020000200002000020000200002000020000200002000020000",
                        Password = "666666",
                        RoleIds = new List<long>() { userRole.Id }
                    }
                }
            };
        }

        [Theory]
        [MemberData(nameof(CreateUserIncorrectDataParameters))]
        public async Task CreateUser_IncorrectData_ReturnsErrorsList(
            EditUserDto editUserDto,
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

            (HttpStatusCode createUserHttpStatusCode, List<ErrorInfo>? createUserErrorResponse) =
                await RequestService.SendPostAsync<EditUserDto, List<ErrorInfo>>(
                    UsersUrl,
                    editUserDto,
                    HttpClient,
                    authenticateResponse.AccessToken
                );
            (HttpStatusCode authenticateHttpStatusCode, _) = await RequestService.SendPostAsync
                <AuthenticateRequest, AuthenticateResponse>(
                    AuthenticateUrl,
                    new AuthenticateRequest
                    {
                        Email = editUserDto.Email ?? "",
                        Password = editUserDto.Password ?? ""
                    },
                    HttpClient
                );

            Assert.Equal(HttpStatusCode.BadRequest, createUserHttpStatusCode);
            createUserErrorResponse.Should().BeEquivalentTo(expectedErrors);
            Assert.Equal(HttpStatusCode.Unauthorized, authenticateHttpStatusCode);
        }

        public static IEnumerable<object[]> CreateUserIncorrectDataParameters()
        {
            Role adminRole = new Roles().Data["admin"];
            return new List<object[]>
            {
                new object[]
                {
                    new EditUserDto()
                    {
                        Email = null,
                        FirstName = "Testowy",
                        LastName = "Tester",
                        Password = "300300",
                        RoleIds = new List<long>() { adminRole.Id }
                    },
                    new List<ErrorInfo>()
                    {
                        new ErrorInfo(errorKey: "IsRequired", elementKey: "Email")
                    }
                },
                new object[]
                {
                    new EditUserDto()
                    {
                        Email = "",
                        FirstName = "Testowy",
                        LastName = "Tester",
                        Password = "300300",
                        RoleIds = new List<long>() { adminRole.Id }
                    },
                    new List<ErrorInfo>()
                    {
                        new ErrorInfo(errorKey: "IsRequired", elementKey: "Email")
                    }
                },
                new object[]
                {
                    new EditUserDto()
                    {
                        Email = "  ",
                        FirstName = "Testowy",
                        LastName = "Tester",
                        Password = "300300",
                        RoleIds = new List<long>() { adminRole.Id }
                    },
                    new List<ErrorInfo>()
                    {
                        new ErrorInfo(errorKey: "IsRequired", elementKey: "Email")
                    }
                },
                new object[]
                {
                    new EditUserDto()
                    {
                        Email = "test",
                        FirstName = "Testowy",
                        LastName = "Tester",
                        Password = "300300",
                        RoleIds = new List<long>() { adminRole.Id }
                    },
                    new List<ErrorInfo>()
                    {
                        new ErrorInfo(errorKey: "WrongStructure", elementKey: "Email")
                    }
                },
                new object[]
                {
                    new EditUserDto()
                    {
                        Email = "test200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200@gmail.com",
                        FirstName = "Testowy",
                        LastName = "Tester",
                        Password = "300300",
                        RoleIds = new List<long>() { adminRole.Id }
                    },
                    new List<ErrorInfo>()
                    {
                        new ErrorInfo(errorKey: "TooLong", elementKey: "Email")
                    }
                },
                new object[]
                {
                    new EditUserDto()
                    {
                        Email = "test@lukaszrydzkowski.pl",
                        FirstName = "Testowy",
                        LastName = "Tester",
                        Password = "300300",
                        RoleIds = new List<long>() { adminRole.Id }
                    },
                    new List<ErrorInfo>()
                    {
                        new ErrorInfo(errorKey: "Exists", elementKey: "Email")
                    }
                },
                new object[]
                {
                    new EditUserDto()
                    {
                        Email = "test123@gmail.com",
                        FirstName = null,
                        LastName = "Tester",
                        Password = "300300",
                        RoleIds = new List<long>() { adminRole.Id }
                    },
                    new List<ErrorInfo>()
                    {
                        new ErrorInfo(errorKey: "IsRequired", elementKey: "FirstName")
                    }
                },
                new object[]
                {
                    new EditUserDto()
                    {
                        Email = "test123@gmail.com",
                        FirstName = "",
                        LastName = "Tester",
                        Password = "300300",
                        RoleIds = new List<long>() { adminRole.Id }
                    },
                    new List<ErrorInfo>()
                    {
                        new ErrorInfo(errorKey: "IsRequired", elementKey: "FirstName")
                    }
                },
                new object[]
                {
                    new EditUserDto()
                    {
                        Email = "test123@gmail.com",
                        FirstName = "200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200",
                        LastName = "Tester",
                        Password = "300300",
                        RoleIds = new List<long>() { adminRole.Id }
                    },
                    new List<ErrorInfo>()
                    {
                        new ErrorInfo(errorKey: "TooLong", elementKey: "FirstName")
                    }
                },
                new object[]
                {
                    new EditUserDto()
                    {
                        Email = "test123@gmail.com",
                        FirstName = "Testowy",
                        LastName = null,
                        Password = "300300",
                        RoleIds = new List<long>() { adminRole.Id }
                    },
                    new List<ErrorInfo>()
                    {
                        new ErrorInfo(errorKey: "IsRequired", elementKey: "LastName")
                    }
                },
                new object[]
                {
                    new EditUserDto()
                    {
                        Email = "test123@gmail.com",
                        FirstName = "Testowy",
                        LastName = "",
                        Password = "300300",
                        RoleIds = new List<long>() { adminRole.Id }
                    },
                    new List<ErrorInfo>()
                    {
                        new ErrorInfo(errorKey: "IsRequired", elementKey: "LastName")
                    }
                },
                new object[]
                {
                    new EditUserDto()
                    {
                        Email = "test123@gmail.com",
                        FirstName = "Testowy",
                        LastName = "200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200",
                        Password = "300300",
                        RoleIds = new List<long>() { adminRole.Id }
                    },
                    new List<ErrorInfo>()
                    {
                        new ErrorInfo(errorKey: "TooLong", elementKey: "LastName")
                    }
                },
                new object[]
                {
                    new EditUserDto()
                    {
                        Email = "test123@gmail.com",
                        FirstName = "Testowy",
                        LastName = "Tester",
                        Password = null,
                        RoleIds = new List<long>() { adminRole.Id }
                    },
                    new List<ErrorInfo>()
                    {
                        new ErrorInfo(errorKey: "IsRequired", elementKey: "Password")
                    }
                },
                new object[]
                {
                    new EditUserDto()
                    {
                        Email = "test123@gmail.com",
                        FirstName = "Testowy",
                        LastName = "Tester",
                        Password = "",
                        RoleIds = new List<long>() { adminRole.Id }
                    },
                    new List<ErrorInfo>()
                    {
                        new ErrorInfo(errorKey: "IsRequired", elementKey: "Password")
                    }
                },
                new object[]
                {
                    new EditUserDto()
                    {
                        Email = "test123@gmail.com",
                        FirstName = "Testowy",
                        LastName = "Tester",
                        Password = "t22r3",
                        RoleIds = new List<long>() { adminRole.Id }
                    },
                    new List<ErrorInfo>()
                    {
                        new ErrorInfo(errorKey: "TooShort", elementKey: "Password")
                    }
                },
                new object[]
                {
                    new EditUserDto()
                    {
                        Email = "test123@gmail.com",
                        FirstName = "Testowy",
                        LastName = "Tester",
                        Password = "1231231231231231231231231231231",
                        RoleIds = new List<long>() { adminRole.Id }
                    },
                    new List<ErrorInfo>()
                    {
                        new ErrorInfo(errorKey: "TooLong", elementKey: "Password")
                    }
                },
                new object[]
                {
                    new EditUserDto()
                    {
                        Email = "test123@gmail.com",
                        FirstName = "Testowy",
                        LastName = "Tester",
                        Password = "123123",
                        RoleIds = new List<long>() { }
                    },
                    new List<ErrorInfo>()
                    {
                        new ErrorInfo(errorKey: "IsRequired", elementKey: "RoleId")
                    }
                },
                new object[]
                {
                    new EditUserDto()
                    {
                        Email = "test123@gmail.com",
                        FirstName = "Testowy",
                        LastName = "Tester",
                        Password = "123123",
                        RoleIds = null
                    },
                    new List<ErrorInfo>()
                    {
                        new ErrorInfo(errorKey: "IsRequired", elementKey: "RoleId")
                    }
                },
                new object[]
                {
                    new EditUserDto()
                    {
                        Email = "test123@gmail.com",
                        FirstName = "Testowy",
                        LastName = "Tester",
                        Password = "123123",
                        RoleIds = new List<long>() { 3 }
                    },
                    new List<ErrorInfo>()
                    {
                        new ErrorInfo(
                            errorKey: "NotExist",
                            elementKey: "RoleId",
                            data: new Dictionary<string, string>() { { "RoleId", "3" } }
                        )
                    }
                },
                new object[]
                {
                    new EditUserDto(),
                    new List<ErrorInfo>()
                    {
                        new ErrorInfo(errorKey: "IsRequired", elementKey: "Email"),
                        new ErrorInfo(errorKey: "IsRequired", elementKey: "FirstName"),
                        new ErrorInfo(errorKey: "IsRequired", elementKey: "LastName"),
                        new ErrorInfo(errorKey: "IsRequired", elementKey: "Password"),
                        new ErrorInfo(errorKey: "IsRequired", elementKey: "RoleId")
                    }
                },
                new object[]
                {
                    new EditUserDto()
                    {
                        Email = "test123gmail.com",
                        FirstName = "200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200200",
                        LastName = "",
                        Password = "1231231231231231231231231231231",
                        RoleIds = new List<long>() { adminRole.Id, 3 }
                    },
                    new List<ErrorInfo>()
                    {
                        new ErrorInfo(errorKey: "WrongStructure", elementKey: "Email"),
                        new ErrorInfo(errorKey: "TooLong", elementKey: "FirstName"),
                        new ErrorInfo(errorKey: "IsRequired", elementKey: "LastName"),
                        new ErrorInfo(errorKey: "TooLong", elementKey: "Password"),
                        new ErrorInfo(
                            errorKey: "NotExist",
                            elementKey: "RoleId",
                            data: new Dictionary<string, string>() { { "RoleId", "3" } }
                        )
                    }
                }
            };
        }
    }
}
