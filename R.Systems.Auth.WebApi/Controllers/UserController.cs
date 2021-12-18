using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using R.Systems.Auth.Core.Models;
using R.Systems.Auth.Core.Services;
using R.Systems.Auth.SharedKernel.Jwt;
using R.Systems.Auth.SharedKernel.Validation;
using R.Systems.Auth.WebApi.Features.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R.Systems.Auth.WebApi.Controllers
{
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
        public async Task<IActionResult> Get(long userId)
        {
            UserDto? user = await UserReadService.GetAsync(userId);
            if (user == null)
            {
                return NotFound(null);
            }
            return Ok(user);
        }

        [HttpGet, Authorize(Roles = "admin")]
        public async Task<IActionResult> Get()
        {
            List<UserDto> users = await UserReadService.GetAsync();
            return Ok(users);
        }

        [HttpPost, Authorize(Roles = "admin")]
        public async Task<IActionResult> Create(EditUserDto editUserDto)
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
        public async Task<IActionResult> Update(EditUserDto editUserDto, long userId)
        {
            OperationResult<long> operationResult = await UserWriteService.EditUserAsync(editUserDto, userId);
            if (!operationResult.Result)
            {
                return BadRequest(ValidationResult.Errors);
            }
            return Ok();
        }

        [HttpDelete, Route("{userId}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(long userId)
        {
            long authorizedUserId = UserClaimsService.GetUserId();
            bool result = await UserWriteService.DeleteUserAsync(userId, authorizedUserId);
            if (!result)
            {
                return BadRequest(ValidationResult.Errors);
            }
            return Ok();
        }
    }
}
