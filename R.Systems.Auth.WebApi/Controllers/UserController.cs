using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using R.Systems.Auth.Core.Models.Users;
using R.Systems.Auth.Core.Services;
using R.Systems.Auth.WebApi.Features.User;
using R.Systems.Shared.Core.Validation;
using R.Systems.Shared.WebApi.Jwt;

namespace R.Systems.Auth.WebApi.Controllers;

[ApiController, Route("users")]
public class UserController : ControllerBase
{
    public UserController(
        UserReadService userReadService,
        UserWriteService userWriteService,
        ValidationResult validationResult,
        UserClaimsService userClaimsService)
    {
        UserReadService = userReadService;
        UserWriteService = userWriteService;
        ValidationResult = validationResult;
        UserClaimsService = userClaimsService;
    }

    public UserReadService UserReadService { get; }
    public UserWriteService UserWriteService { get; }
    public ValidationResult ValidationResult { get; }
    public UserClaimsService UserClaimsService { get; }

    [HttpGet, Route("{userId}"), Authorize(Roles = "admin")]
    public async Task<IActionResult> GetUser(long userId)
    {
        UserDto? user = await UserReadService.GetAsync(userId);
        if (user == null)
        {
            return NotFound(null);
        }
        return Ok(user);
    }

    [HttpGet, Authorize(Roles = "admin")]
    public async Task<IActionResult> GetUsers()
    {
        List<UserDto> users = await UserReadService.GetAsync();
        return Ok(users);
    }

    [HttpPost, Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateUser(EditUserDto editUserDto)
    {
        OperationResult<long> operationResult = await UserWriteService.EditUserAsync(editUserDto);
        if (!operationResult.Result)
        {
            return BadRequest(ValidationResult.Errors);
        }
        return Ok(new CreateUserResponse()
        {
            UserId = operationResult.Data
        });
    }

    [HttpPost, Route("{userId}"), Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdateUser(EditUserDto editUserDto, long userId)
    {
        OperationResult<long> operationResult = await UserWriteService.EditUserAsync(editUserDto, userId);
        if (!operationResult.Result)
        {
            return BadRequest(ValidationResult.Errors);
        }
        return Ok();
    }

    [HttpDelete, Route("{userId}"), Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteUser(long userId)
    {
        long authorizedUserId = UserClaimsService.GetUserId();
        bool result = await UserWriteService.DeleteUserAsync(userId, authorizedUserId);
        if (!result)
        {
            return BadRequest(ValidationResult.Errors);
        }
        return Ok();
    }

    [HttpPost, Route("change-password"), Authorize]
    public async Task<IActionResult> ChangeUsersPassword(ChangePasswordRequest request)
    {
        long userId = UserClaimsService.GetUserId();
        bool result = await UserWriteService.ChangeUserPasswordAsync(
            userId, request.CurrentPassword, request.NewPassword, request.RepeatedNewPassword
        );
        if (!result)
        {
            return BadRequest(ValidationResult.Errors);
        }
        return Ok();
    }
}
